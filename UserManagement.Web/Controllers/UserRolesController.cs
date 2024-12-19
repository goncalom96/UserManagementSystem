using System;
using System.Linq;
using System.Web.Mvc;
using UserManagement.DAL.Models.Users;
using UserManagement.DAL.Repository;
using UserManagement.Web.Filters;

namespace UserManagement.Web.Controllers
{
    [CustomAuthorize(Roles = "Administrator")]
    public class UserRolesController : Controller
    {
        private readonly UnitOfWork uow;

        public UserRolesController()
        {
            uow = new UnitOfWork();
        }

        [HttpGet]
        public ActionResult ManageRoles()
        {
            try
            {
                IQueryable<UserRole> roles = uow.UserRoleRepository.GetRoles();
                return View(roles);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"There was an error loading the data: {ex.Message}";
                return View("_Error");
            }
        }

        [HttpGet]
        public ActionResult Create()
        {
            UserRole userRole = new UserRole();

            return View(userRole);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UserRole userRole)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    UserRole roleExist = uow.UserRoleRepository.GetRole(r => r.RoleType == userRole.RoleType);

                    // Verificar se o role já existe
                    if (roleExist != null)
                    {
                        ModelState.AddModelError("RoleType", "Role already exists.");

                        // Retorna a View com os dados preenchidos
                        return View(userRole);
                    }

                    // Guardar o novo userRole
                    uow.UserRoleRepository.Create(userRole);
                    uow.SaveChanges();

                    // Retorna JSON com success = true
                    return Json(new { success = true, message = "Saved Successfully" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"An error occurred while creating new role: {ex.Message}";
                    return View("_Error");
                }
            }
            return View(userRole);
        }

        [HttpGet]
        public ActionResult Update(int id)
        {
            try
            {
                UserRole userRole = uow.UserRoleRepository.GetRoleById(id);

                if (userRole == null)
                {
                    return HttpNotFound();
                }

                return View(userRole);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"There was an error loading the data: {ex.Message}";
                return View("_Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(UserRole userRole)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    UserRole roleExist = uow.UserRoleRepository.GetRole(r => r.RoleType == userRole.RoleType);

                    // Verifica se userRole já existe
                    if (roleExist != null)
                    {
                        ModelState.AddModelError("RoleType", "Role already exists.");

                        return View(userRole);
                    }

                    uow.UserRoleRepository.Update(userRole);
                    uow.SaveChanges();

                    return Json(new { success = true, message = "Updated Successfully" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"An error occurred while updating the role: {ex.Message}";
                    return View("_Error");
                }
            }

            return View(userRole);
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            try
            {
                UserRole userRole = uow.UserRoleRepository.GetRoleById(id);

                if (userRole == null)
                {
                    return HttpNotFound();
                }

                return View(userRole);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"There was an error loading the data: {ex.Message}";
                return View("_Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(UserRole userRole)
        {
            try
            {
                UserRole roleExist = uow.UserRoleRepository.GetRole(r => r.UserRoleId == userRole.UserRoleId && r.RoleType == userRole.RoleType);

                // Verifica se userRole existe
                if (roleExist != null)
                {
                    uow.UserRoleRepository.Delete(roleExist.UserRoleId);
                    uow.SaveChanges();

                    return Json(new { success = true, message = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);
                }

                return HttpNotFound();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred while deleting the role: {ex.Message}";
                return View("_Error");
            };
        }
    }
}