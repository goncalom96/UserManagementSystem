using System.Web.Mvc;

namespace UserManagement.Web.Controllers
{
    public class ErrorHandlerController : Controller
    {
        public ActionResult Index(int code)
        {
            switch (code)
            {
                case 400:
                    TempData["ErrorTitle"] = "Bad request";
                    TempData["ErrorMessage"] = "Invalid request.";
                    break;

                case 401:
                    TempData["ErrorTitle"] = "Unauthorized";
                    TempData["ErrorMessage"] = "Login required.";
                    break;

                case 403:
                    TempData["ErrorTitle"] = "Forbidden";
                    TempData["ErrorMessage"] = "Access denied.";
                    break;

                case 404:
                    TempData["ErrorTitle"] = "Not Found";
                    TempData["ErrorMessage"] = "Page not found.";
                    break;

                case 500:
                    TempData["ErrorTitle"] = "Internal Server Error";
                    TempData["ErrorMessage"] = "An internal server error occurred.";
                    break;

                case 503:
                    TempData["ErrorTitle"] = "Service Unavailable";
                    TempData["ErrorMessage"] = "The service is temporarily unavailable.";
                    break;

                default:
                    TempData["ErrorTitle"] = "Unexpected Error";
                    TempData["ErrorMessage"] = "An unexpected error occurred.";
                    break;
            }
            return View("_Error");
        }

        public ActionResult AccessDenied()
        {
            TempData["ErrorTitle"] = "Access denied";
            TempData["ErrorMessage"] = "Unauthorized page.";
            return View("_Error");
        }
    }
}