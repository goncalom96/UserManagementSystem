using System;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace UserManagement.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_Error()
        {
            Exception exception = Server.GetLastError();
            Server.ClearError();

            var httpException = exception as HttpException;
            int code = httpException?.GetHttpCode() ?? 500; // Se não for HttpException, trata como erro 500

            Response.Redirect(url: $"/ErrorHandler/Index?code={code}");
        }

        protected void Application_PostAuthenticateRequest(object sender, EventArgs e)
        {
            // Ler o cookie de autenticação FormsAuthentication
            HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];

            if (authCookie != null)
            {
                // Decifrar o ticket do cookie
                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);

                if (authTicket != null && !authTicket.Expired)
                {
                    // Obter os roles do UserData (armazenados como string)
                    string[] roles = authTicket.UserData.Split(',');

                    // Criar uma identidade do usuário com os roles e atribuí-la ao HttpContext
                    GenericPrincipal userPrincipal = new GenericPrincipal(new GenericIdentity(authTicket.Name), roles);

                    // Atribuir ao contexto atual
                    HttpContext.Current.User = userPrincipal;
                }
            }
        }
    }
}