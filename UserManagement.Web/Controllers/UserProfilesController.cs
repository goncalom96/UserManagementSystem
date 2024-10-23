using System;
using System.Web.Mvc;
using UserManagement.DAL.Models.Users;
using UserManagement.DAL.Repository;

namespace UserManagement.Web.Controllers
{
    public class UserProfilesController : Controller
    {
        private readonly UnitOfWork uow;

        public UserProfilesController()
        {
            uow = new UnitOfWork();
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Create(int userLoginId)
        {
            // Cria um novo UserProfile e associa o UserLoginId
            UserProfile userProfile = new UserProfile
            {
                UserLoginId = userLoginId
            };
            return View(userProfile);
        }

        //[Authorize(Roles = "Administrator, Guest")]
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UserProfile userProfile)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    userProfile.LastModified = DateTime.Now;

                    uow.UserProfileRepository.Create(userProfile);
                    uow.SaveChanges();

                    return RedirectToAction("Login", "UserLogins");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Registration failed: {ex.Message}";
                    return View("Error");
                }
            }

            // Retorna a View com os dados preenchidos e erros de validação, se houver
            return View(userProfile);
        }

        [HttpGet]
        [Authorize]
        public ActionResult Details()
        {
            try
            {
                UserLogin userLogin = uow.UserLoginRepository.GetUser(u => u.UserName == User.Identity.Name);

                if (userLogin == null)
                {
                    return RedirectToAction("Register", "UserLogins");
                }
                
                UserProfile userProfile = uow.UserProfileRepository.GetUserProfile(u => u.UserLoginId == userLogin.UserLoginId);

                if (userProfile != null)
                {
                    return View(userProfile);
                }
                else
                {
                    return RedirectToAction(actionName: "Edit", "UserProfiles");
                }
            }
            catch (Exception ex)
            {
                // Aqui você pode registrar a exceção e redirecionar ou retornar uma View de erro
                ModelState.AddModelError("", $"An error occurred while retrieving user details: {ex.Message}");
                return View("Error"); // Ou redirecione para uma página de erro            }
            }
        }
    }
}