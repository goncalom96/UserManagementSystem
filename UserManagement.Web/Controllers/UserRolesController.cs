using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UserManagement.DAL.Models.Users;
using UserManagement.DAL.Repository;

namespace UserManagement.Web.Controllers
{
    public class UserRolesController : Controller
    {
        private readonly UnitOfWork uow;

        public UserRolesController()
        {
            uow = new UnitOfWork();
        }

        [Authorize(Roles = "Administrator")]
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
    }
}