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
                TempData["ErrorMessage"] = $"An error occurred while retrieving roles: {ex.Message}";
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult Create(UserRole userRole)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Verificar se o role já existe
                    UserRole roleExist = uow.UserRoleRepository.GetRole(r => r.RoleType == userRole.RoleType);

                    // Verificações de dados existentes do role
                    if (roleExist != null)
                    {
                        // Verificações de dados existentes do role
                        if (roleExist.RoleType == userRole.RoleType)
                        {
                            ModelState.AddModelError("RoleType", "Role already exists.");
                        }

                        // Retorna a View com os dados preenchidos
                        return View(userRole);
                    }
                    else
                    {
                        // Guardar o novo userRole
                        uow.UserRoleRepository.Create(userRole);
                        uow.SaveChanges();

                        return View();
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Role registration failed: {ex.Message}";
                    return View("Error");
                }
            }
            return View(userRole);
        }

        [HttpPost]
        public ActionResult Edit(int id)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    UserRole userRole = uow.UserRoleRepository.GetRoleById(id);

                    if (userRole != null)
                    {
                        uow.UserRoleRepository.Edit(userRole);
                        uow.SaveChanges();

                        return View();
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Role not found!";
                        return View("Error");
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Role registration failed: {ex.Message}";
                    return View("Error");
                }
            }

            return View();
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                UserRole userRole = uow.UserRoleRepository.GetRoleById(id);

                if (userRole != null)
                {
                    uow.UserRoleRepository.Delete(userRole);
                    uow.SaveChanges();
                    return View();
                }
                else
                {
                    TempData["ErrorMessage"] = "Role not found!";
                    return View("Error");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Failure to delete role: {ex.Message}";
                return View("Error");
            }
        }
    }
}