using System.Web.Mvc;

namespace UserManagement.Web.Controllers
{
    public class ErrorHandlerController : Controller
    {
        public ActionResult Index(int code)
        {
            switch (code)
            {
                case 403:
                    ViewBag.ErrorTitle = "Access denied";
                    TempData["ErrorMessage"] = "Unauthorized page.";
                    break;

                case 404:
                    ViewBag.ErrorTitle = "Page Not Found";
                    TempData["ErrorMessage"] = "The page you are looking for does not exist.";
                    break;

                case 500:
                    ViewBag.ErrorTitle = "Internal Server Error";
                    TempData["ErrorMessage"] = "An internal server error occurred.";
                    break;

                default:
                    ViewBag.ErrorTitle = "Unexpected Error";
                    TempData["ErrorMessage"] = "An unexpected error occurred.";
                    break;
            }

            return View("Error");
        }
    }
}