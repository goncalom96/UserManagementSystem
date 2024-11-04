using System;
using System.Web.Mvc;
using UserManagement.DAL.Models.Users;
using UserManagement.DAL.Repository;
using UserManagement.Web.Models;
using UserManagement.Web.Services;

namespace UserManagement.Web.Controllers
{
    public class UserProfilesController : Controller
    {
        private readonly UnitOfWork uow;
        private readonly ImageService imageService;

        public UserProfilesController()
        {
            uow = new UnitOfWork();
            imageService = new ImageService("/Assets/Images/Users/");
        }

        [HttpGet]
        public ActionResult Create(int userLoginId)
        {
            // Cria um novo profile e associa o UserLoginId
            ProfileViewModel profile = new ProfileViewModel
            {
                UserLoginId = userLoginId,
                DateOfBirth = new DateTime(1990, 1, 1)
            };
            return View(profile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProfileViewModel profile)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Caminho relativo da imagem usado para armazenar no banco e referenciar a imagem ao exibi-la na aplicação.
                    string imageUrl = imageService.SaveImage(profile.ImageFile);

                    // Novo userProfile
                    UserProfile userProfile = new UserProfile()
                    {
                        UserLoginId = profile.UserLoginId,
                        FirstName = profile.FirstName,
                        LastName = profile.LastName,
                        DateOfBirth = profile.DateOfBirth,
                        Gender = profile.Gender,
                        ImageUrl = imageUrl,
                        LastModified = DateTime.Now,
                    };

                    // Criação do perfil
                    uow.UserProfileRepository.Create(userProfile);
                    uow.SaveChanges();

                    TempData["ConfirmationMessage"] = "Your account has been successfully created.";
                    return View(viewName: "ConfirmationMessage");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Registration failed: {ex.Message}";
                    return View("Error");
                }
            }

            // Retorna a View com os dados preenchidos
            return View(profile);
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
                ModelState.AddModelError("", $"An error occurred while retrieving user details: {ex.Message}");
                return View("Error");           }
            }
        }
    }
}