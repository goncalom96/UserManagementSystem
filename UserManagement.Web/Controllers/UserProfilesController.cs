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
        public ActionResult Create(int userLoginId)
        {
            // Cria um novo UserProfile e associa o UserLoginId
            UserProfile userProfile = new UserProfile
            {
                UserLoginId = userLoginId,
                DateOfBirth = new DateTime(1990, 1, 1)
            };
            return View(userProfile);
        }

        [HttpPost]
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

                    TempData["ConfirmationMessage"] = "Your account has been successfully created.";
                    return View("ConfirmationMessage");
                }
                catch (Exception ex)
                {
                    uow.UserLoginRepository.Delete(userProfile.UserLoginId);
                    uow.SaveChanges();
                    TempData["ErrorMessage"] = $"Registration failed: {ex.Message}";
                    return View("Error");
                }
            }

            // Retorna a View com os dados preenchidos
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