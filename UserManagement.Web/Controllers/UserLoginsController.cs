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

        // Injetação do UnitOfWork via construtor
        // Segue o princípio da Inversão de Controle(IoC), facilitando a substituição do contexto(para fins de testes, por exemplo)
        //public UserLoginsController(UnitOfWork uow)
        //{
        //    this.uow = uow; // Instância direta do UnitOfWork
        //}

        //LOGIN COM FORMS AUTHENTICATION

        [HttpGet]
        public ActionResult Login()
        {
            LoginViewModel login = new LoginViewModel();
            return View(login);
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // Token para proteger contra CSRF
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
                    uow.SaveChanges();

                    // Opção 1
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

                    #region Outras opções

                    // Opção 2 (sem role) - Configura o cookie de autenticação
                    // false: Este parâmetro indica se o cookie deve ser persistente. Quando false, o cookie será um cookie de sessão, ou seja, ele será válido apenas durante a sessão do navegador atual. Quando o usuário fechar o navegador, o cookie será descartado.
                    //FormsAuthentication.SetAuthCookie(userLogin.UserName, false);

                    // Opção 3
                    //FormsAuthentication.SetAuthCookie(userLogin.UserName, true);
                    //Session.Add("userole", userLogin.UserRole.RoleType.ToString());

                    #endregion Outras opções

                    return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Failed to Log In: {ex.Message}";
                    return View("_Error");
                }
            }

            // Se chegou aqui, significa que houve um erro de validação ou autenticação falhou
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
                    // Verificar se o user já existe
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

                    // Associar o userRegister a um novo UserLogin
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

                    // Criação do UserProfile
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

        public ActionResult ResetPassword(string email, string token)
        {
            UserLogin userLogin = uow.UserLoginRepository.GetUser(u => u.EmailAddress == email && u.PasswordRecoveryToken == token);

            if (userLogin != null)
            {
                ResetPasswordViewModel model = new ResetPasswordViewModel
                {
                    ResetToken = token
                };
                return View(model);
            }
            return View("_Error");
        }

        [HttpPost]
        public ActionResult ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                UserLogin userLogin = uow.UserLoginRepository.GetUser(u => u.PasswordRecoveryToken == model.ResetToken);

                if (userLogin != null)
                {
                    userLogin.Password = model.NewPassword;
                    userLogin.PasswordRecoveryToken = null;
                    uow.SaveChanges();

                    TempData["ConfirmationMessage"] = "Your password has been successfully changed.";
                    return View("_ConfirmationMessage");
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
                else
                {
                    return View(userLogin);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred while retrieving user details: {ex.Message}");
                return View("_Error"); // Ou redirecione para uma página de erro
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
                TempData["ErrorMessage"] = $"An error occurred while retrieving users: {ex.Message}";
                return View("_Error");
            }

            #region Outras opções

            // Opção 2 - Ajustar Construtor
            //IEnumerable<UserLogin> users = uow.UserLoginRepository.GetUsers2();
            //return View(users);

            // Opção 3
            // Com esta abordagem não é necessário do método dispose
            //using (UnitOfWork uow = new UnitOfWork())
            //{
            //    IQueryable<UserLogin> users = uow.UserLoginRepository.GetUsers().ToList().AsQueryable();

            //    return View(users);
            //}

            #endregion Outras opções
        }

        [HttpGet]
        [CustomAuthorize(Roles = "Administrator")]
        public ActionResult Create()
        {
            // Roles existentes na BD
            IEnumerable<UserRole> roles = uow.UserRoleRepository.GetRoles();

            // A classe SelectList é usada para criar uma lista de itens que será associada a um DropDownList na View.
            // Collection: A lista de itens que você está passando(no seu caso, a lista de papéis roles).
            // ValueField: O campo que será usado como valor para cada item na dropdown(normalmente o campo que você usa para armazenar o valor selecionado na tabela, como o UserRoleId).
            // TextField: O campo que será exibido como o texto para o usuário na dropdown(no seu caso, o RoleType, que pode ser algo como "Administrador" ou "Usuário").

            // Passa os roles para a ViewBag
            ViewBag.Roles = new SelectList(roles, "UserRoleId", "RoleType");

            UserLogin userLogin = new UserLogin();

            return View(userLogin);
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
                    // Verificar se o user já existe
                    UserLogin userExist = uow.UserLoginRepository.GetUser(u => (u.UserName == userLogin.UserName) || (u.EmailAddress == userLogin.EmailAddress));

                    // Verificações de dados existentes do user
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
            UserLogin userLogin = uow.UserLoginRepository.GetUserById(id);

            if (userLogin == null)
            {
                return HttpNotFound();
            }

            // Roles existentes na BD
            IEnumerable<UserRole> roles = uow.UserRoleRepository.GetRoles();

            // Passa os papéis carregados para a View
            ViewBag.Roles = new SelectList(roles, "UserRoleId", "RoleType");

            return View(userLogin);
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
                    TempData["ErrorMessage"] = $"An error occurred while editing the user: {ex.Message}";
                    return View("_Error");
                }
            }

            return View(userLogin);
        }

        [HttpGet]
        [CustomAuthorize(Roles = "Administrator")]
        public ActionResult Delete(int id)
        {
            UserLogin userLogin = uow.UserLoginRepository.GetUserById(id);

            if (userLogin == null)
            {
                return HttpNotFound();
            }

            return View(userLogin);
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