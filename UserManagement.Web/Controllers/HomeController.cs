using System;
using System.Web.Mvc;

namespace UserManagement.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Failed to access the Home Page: {ex.Message}";
                return View("Error");
            }
        }

        public ActionResult About()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Failed to access the About Page: {ex.Message}";
                return View("Error");
            }
        }
    }
}