using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using UserManagement.DAL.Models.Users;
using UserManagement.DAL.Repository;
using UserManagement.Web.Filters;
using UserManagement.Web.Models;
using UserManagement.Web.Services;

namespace UserManagement.Web.Controllers
{
    public class UserLoginsController : Controller
    {
        private readonly UnitOfWork uow;
        private readonly EmailService emailService;

        // Instância direta do UnitOfWork dentro do construtor
        public UserLoginsController()
        {
            uow = new UnitOfWork();
            emailService = new EmailService();
        }

        //LOGIN COM FORMS AUTHENTICATION
        [HttpGet]
        public ActionResult Login()
        {
            LoginViewModel login = new LoginViewModel();
            return View(login);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel login)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Validação das credenciais
                    UserLogin userLogin = uow.UserLoginRepository.UserValidation(login.UserName, login.Password);

                    if (userLogin == null)
                    {
                        ModelState.AddModelError("", "Incorrect username or password.");
                    }

                    // Autenticação bem-sucedida e o User fica ativo!
                    userLogin.IsActived = true;

                    // Guarda as alterações
                    uow.SaveChanges();

                    // Cria o ticket de autenticação
                    FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                        1,                                      // Versão do ticket
                        userLogin.UserName,                     // Nome do usuário
                        DateTime.Now,                           // Data de emissão
                        DateTime.Now.AddMinutes(30),            // Data de expiração
                        false,                                  // Persistente (false = cookie de sessão)
                        userLogin.UserRole.RoleType.ToString()               // Converte o enum para string e armazena
                    );

                    // Criptografa o ticket
                    string encryptedTicket = FormsAuthentication.Encrypt(authTicket);

                    // Cria o cookie de autenticação
                    HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);

                    // Adiciona o cookie à resposta HTTP
                    HttpContext.Response.Cookies.Add(authCookie);

                    return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Failed to Log In: {ex.Message}";
                    return View("_Error");
                }
            }

            // Retorna a View com os dados preenchidos
            return View(login);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    UserLogin userLogin = uow.UserLoginRepository.GetUser(u => u.UserName == User.Identity.Name);

                    if (userLogin != null)
                    {
                        userLogin.IsActived = false;
                        uow.SaveChanges();

                        FormsAuthentication.SignOut();
                    }
                }

                return RedirectToAction("Login", "UserLogins");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Failed to Log Out: {ex.Message}";
                return View("_Error");
            }
        }

        [HttpGet]
        public ActionResult Register()
        {
            RegisterViewModel userRegister = new RegisterViewModel();
            return View(userRegister);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel userRegister)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Verifica se o user já existe
                    UserLogin userExist = uow.UserLoginRepository.GetUser(u =>
                        (u.UserName == userRegister.UserName) ||
                        (u.EmailAddress == userRegister.EmailAddress));

                    // Verificações de dados existentes do user
                    if (userExist != null)
                    {
                        // Verificações de dados existentes do user
                        if (userExist.UserName == userRegister.UserName)
                        {
                            ModelState.AddModelError("UserName", "Username already exists.");
                        }

                        if (userExist.EmailAddress == userRegister.EmailAddress)
                        {
                            ModelState.AddModelError("EmailAddress", "E-mail already exists.");
                        }

                        // Retorna a View com os dados preenchidos
                        return View(userRegister);
                    }

                    // Associa o userRegister a um novo UserLogin
                    UserLogin userLogin = new UserLogin
                    {
                        UserName = userRegister.UserName,
                        EmailAddress = userRegister.EmailAddress,
                        PhoneNumber = userRegister.PhoneNumber,
                        Password = userRegister.Password,
                        UserRoleId = uow.UserRoleRepository.GetRole(r => r.RoleType == UserRole.EnumRole.Guest).UserRoleId,
                        CreatedAt = DateTime.Now,
                        IsActived = false
                    };

                    // Guardar o novo userLogin
                    uow.UserLoginRepository.Create(userLogin);
                    uow.SaveChanges();

                    // Redireciona para a criação do UserProfile
                    return RedirectToAction("Create", "UserProfiles", new { userLoginId = userLogin.UserLoginId });
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"User registration failed: {ex.Message}";
                    return View("_Error");
                }
            }
            // Retorna a View com os dados preenchidos e erros de validação, se houver
            return View(userRegister);
        }

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(string email)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    UserLogin userLogin = uow.UserLoginRepository.GetUser(u => u.EmailAddress == email);

                    // Verifica se o e-mail existe
                    if (userLogin == null)
                    {
                        ModelState.AddModelError("", "Invalid e-mail address!");
                        return View();
                    }

                    // Gera um token para a recuperação da password
                    string token = Guid.NewGuid().ToString();

                    // Guarda o token na base de dados
                    userLogin.PasswordRecoveryToken = token;
                    uow.SaveChanges();

                    // Criar link de redefinição
                    string resetLink = Url.Action("ResetPassword", "UserLogins", new { Email = email, Token = token }, Request.Url.Scheme);

                    // Enviar email com o link
                    string subject = "Reset password instructions";
                    string body = $"Hey <b>{userLogin.UserName}</b>&#128513;,<br/><br/>Click on the link to reset your password:<br/><br/><a href='{resetLink}'>Reset password</a><br/><br/>Best regards,<br/>UserManagement Team";

                    emailService.SendEmail(email, subject, body);

                    TempData["ConfirmationMessage"] = "Your message has been sent successfully. Check your inbox for a password reset link.";
                    return View("_ConfirmationMessage");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Password change failed: {ex.Message}";
                    return View("_Error");
                }
            }
            return View(email);
        }

        [HttpGet]
        public ActionResult ResetPassword(string email, string token)
        {
            try
            {
                UserLogin userLogin = uow.UserLoginRepository.GetUser(u => u.EmailAddress == email && u.PasswordRecoveryToken == token);

                // Verifica se o user é nulo com base no email e no token
                if (userLogin == null)
                {
                    ModelState.AddModelError("", "Invalid email address or reset token!");
                    return View("_Error");
                }

                ResetPasswordViewModel model = new ResetPasswordViewModel
                {
                    ResetToken = token
                };

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"There was an error loading the data: {ex.Message}";
                return View("_Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    UserLogin userLogin = uow.UserLoginRepository.GetUser(u => u.PasswordRecoveryToken == model.ResetToken);

                    // Verifica o user é nulo com base no token
                    if (userLogin == null)
                    {
                        ModelState.AddModelError("", "The reset token is invalid.");
                        return View("_Error");
                    }

                    // Atualiza a password e remove o token
                    userLogin.Password = model.NewPassword;
                    userLogin.PasswordRecoveryToken = null;

                    // Guarda as alterações
                    uow.SaveChanges();

                    TempData["ConfirmationMessage"] = "Your password has been successfully changed.";
                    return View("_ConfirmationMessage");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"There was an error loading the data: {ex.Message}";
                    return View("_Error");
                }
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult Details()
        {
            try
            {
                UserLogin userLogin = uow.UserLoginRepository.GetUser(u => u.UserName == User.Identity.Name);

                if (userLogin == null)
                {
                    return RedirectToAction("Register", "UserLogins");
                }

                return View(userLogin);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"There was an error loading the data: {ex.Message}";
                return View("_Error");
            }
        }

        [HttpGet]
        [CustomAuthorize(Roles = "Administrator")]
        public ActionResult ManageUsers()
        {
            try
            {
                IQueryable<UserLogin> users = uow.UserLoginRepository.GetUsers();
                return View(users);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"There was an error loading the data: {ex.Message}";
                return View("_Error");
            }
        }

        [HttpGet]
        [CustomAuthorize(Roles = "Administrator")]
        public ActionResult Create()
        {
            try
            {
                // Roles existentes na base de dados
                IEnumerable<UserRole> roles = uow.UserRoleRepository.GetRoles();

                // Passa os roles para a ViewBag
                ViewBag.Roles = new SelectList(roles, "UserRoleId", "RoleType");

                UserLogin userLogin = new UserLogin();

                return View(userLogin);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"There was an error loading the data: {ex.Message}";
                return View("_Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "Administrator")]
        public ActionResult Create(UserLogin userLogin)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    UserLogin userExist = uow.UserLoginRepository.GetUser(u => (u.UserName == userLogin.UserName) || (u.EmailAddress == userLogin.EmailAddress));

                    // Verificar se o user já existe
                    if (userExist != null)
                    {
                        // Verificações de dados existentes do user
                        if (userExist.UserName == userLogin.UserName)
                        {
                            ModelState.AddModelError("UserName", "Username already exists.");
                        }

                        if (userExist.EmailAddress == userLogin.EmailAddress)
                        {
                            ModelState.AddModelError("EmailAddress", "E-mail already exists.");
                        }

                        // Retorna a View com os dados preenchidos
                        return View(userLogin);
                    }

                    // Definir as restantes propriedades
                    userLogin.CreatedAt = DateTime.Now;
                    userLogin.IsActived = false;

                    // Guardar o novo userLogin
                    uow.UserLoginRepository.Create(userLogin);
                    uow.SaveChanges();

                    return Json(new { success = true, message = "Saved Successfully" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"An error occurred while creating new user: {ex.Message}";
                    return View("_Error");
                }
            }
            return View(userLogin);
        }

        [HttpGet]
        [CustomAuthorize(Roles = "Administrator")]
        public ActionResult Update(int id)
        {
            try
            {
                // Verifica se o user existe
                UserLogin userLogin = uow.UserLoginRepository.GetUserById(id);

                if (userLogin == null)
                {
                    return HttpNotFound();
                }

                // Roles existentes na base de dados
                IEnumerable<UserRole> roles = uow.UserRoleRepository.GetRoles();

                // Passa os papéis carregados para a View
                ViewBag.Roles = new SelectList(roles, "UserRoleId", "RoleType");

                return View(userLogin);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"There was an error loading the data: {ex.Message}";
                return View("_Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "Administrator")]
        public ActionResult Update(UserLogin userLogin)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    uow.UserLoginRepository.Update(userLogin);
                    uow.SaveChanges();

                    return Json(new { success = true, message = "Updated Successfully" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"An error occurred while updating user: {ex.Message}";
                    return View("_Error");
                }
            }

            return View(userLogin);
        }

        [HttpGet]
        [CustomAuthorize(Roles = "Administrator")]
        public ActionResult Delete(int id)
        {
            try
            {
                UserLogin userLogin = uow.UserLoginRepository.GetUserById(id);

                if (userLogin == null)
                {
                    return HttpNotFound();
                }

                return View(userLogin);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"There was an error loading the data: {ex.Message}";
                return View("_Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "Administrator")]
        public ActionResult Delete(UserLogin userLogin)
        {
            try
            {
                uow.UserLoginRepository.Delete(userLogin.UserLoginId);
                uow.SaveChanges();

                return Json(new { success = true, message = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred while deleting the user: {ex.Message}";
                return View("_Error");
            };
        }
    }
}