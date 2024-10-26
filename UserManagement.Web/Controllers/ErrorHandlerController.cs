using System.Web.Mvc;

namespace UserManagement.Web.Controllers
{
    public class ErrorHandlerController : Controller
    {
        public ActionResult Index(int code)
        {
            switch (code)
            {
                case 401:
                case 403:
                    TempData["ErrorTitle"] = "Access denied";
                    TempData["ErrorMessage"] = "Unauthorized page.";
                    return RedirectToAction("AccessDenied", "ErrorHandler");

                case 404:
                    TempData["ErrorTitle"] = "Page Not Found";
                    TempData["ErrorMessage"] = "The page you are looking for does not exist.";
                    break;

                case 500:
                    TempData["ErrorTitle"] = "Internal Server Error";
                    TempData["ErrorMessage"] = "An internal server error occurred.";
                    break;

                default:
                    TempData["ErrorTitle"] = "Unexpected Error";
                    TempData["ErrorMessage"] = "An unexpected error occurred.";
                    break;
            }
            return View("Error");
        }

        public ActionResult AccessDenied()
        {
            TempData["ErrorTitle"] = "Access denied";
            TempData["ErrorMessage"] = "Unauthorized page.";
            return View("Error");
        }
    }
}