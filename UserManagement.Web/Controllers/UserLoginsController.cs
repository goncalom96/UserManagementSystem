using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using UserManagement.DAL.Models.Users;
using UserManagement.DAL.Repository;
using UserManagement.Web.Filters;
using UserManagement.Web.Models;

namespace UserManagement.Web.Controllers
{
    public class UserLoginsController : Controller
    {
        private readonly UnitOfWork uow;

        // Instância direta do UnitOfWork dentro do construtor
        public UserLoginsController()
        {
            uow = new UnitOfWork();
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

        // POST envia dados para o servidor para serem processados, geralmente do corpo da requisição.
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

                    if (userLogin != null)
                    {
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
                    else
                    {
                        ModelState.AddModelError("", "Incorrect username or password.");
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Failed to Log In: {ex.Message}";
                    return View("Error");
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
                return View("Error");
            }
        }

        [HttpGet]
        public ActionResult Register()
        {
            UserLogin userLogin = new UserLogin();
            return View(userLogin);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(UserLogin userLogin)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    UserLogin userLoginExist = uow.UserLoginRepository.GetUser(u => (u.UserName == userLogin.UserName) || (u.Email == userLogin.Email));

                    if (userLoginExist != null)
                    {
                        if (userLoginExist.UserName == userLogin.UserName && userLoginExist.Email != userLogin.Email)
                        {
                            ModelState.AddModelError("UserName", errorMessage: "Username already exists.");
                        }

                        if (userLoginExist.Email == userLogin.Email && userLoginExist.UserName != userLogin.UserName)
                        {
                            ModelState.AddModelError("Email", "E-mail already exists.");
                        }

                        if (userLoginExist.UserName == userLogin.UserName && userLoginExist.Email == userLogin.Email)
                        {
                            ModelState.AddModelError("UserName", errorMessage: "Username already exists.");
                            ModelState.AddModelError("Email", "E-mail already exists.");
                        }

                        return View(userLogin); // Retorna a View com os dados preenchidos
                    }

                    userLogin.UserRoleId = uow.UserRoleRepository.GetRole(r => r.RoleType == UserRole.EnumRole.Guest).UserRoleId;
                    userLogin.CreatedAt = DateTime.Now;
                    userLogin.IsActived = false;

                    uow.UserLoginRepository.Create(userLogin);
                    uow.SaveChanges(); // O User é guardado apenas no UserProfiles/Create

                    return RedirectToAction("Create", "UserProfiles", new { userLoginId = userLogin.UserLoginId });
                }
                catch (Exception ex) // Captura a exceção
                {
                    TempData["ErrorMessage"] = $"User registration failed: {ex.Message}";
                    return View("Error");
                }
            }

            // Retorna a View com os dados preenchidos e erros de validação, se houver
            return View(userLogin);
        }

        [HttpGet]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    ModelState.AddModelError("", "Please enter your Email.");
                    return View();
                }

                UserLogin userLogin = uow.UserLoginRepository.GetUser(u => u.Email == email);

                if (userLogin == null)
                {
                    ModelState.AddModelError("", "User not found.");
                    return View();
                }

                //string resetToken = Guid.NewGuid().ToString();
                //userLogin.PasswordResetToken = resetToken;
                //userLogin.ResetTokenExpiration = DateTime.Now.AddHours(1); // Token expira em 1 hora
                uow.SaveChanges();

                // Aqui envias o email com o link de recuperação
                TempData["Message"] = "Check your email for the password reset link.";
                return RedirectToAction("ForgotPasswordConfirmation");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Password change failed: {ex.Message}";
                return View("Error");
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
                return View("Error");
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
    }
}