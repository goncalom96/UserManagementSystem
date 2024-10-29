using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace UserManagement.Web.Filters
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (HttpContext.Current.User == null || !HttpContext.Current.User.Identity.IsAuthenticated)
            {
                // User não autenticado, redireciona para a página de login
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
                {
                    { "Controller", "UserLogins" },
                    { "Action", "Login" }
                });
            }
            else
            {
                // User autenticado, mas sem a permissão necessária (sem o role correto)
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
                {
                    { "Controller", "ErrorHandler" },
                    { "Action", "AccessDenied" }
                });
            }
        }
    }
}