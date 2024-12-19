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

        // Responsável por capturar e tratar erros não gerenciados que ocorrem em toda a aplicação.
        protected void Application_Error()
        {
            // Obtém a última exceção que ocorreu durante o processamento da solicitação
            Exception exception = Server.GetLastError();

            Server.ClearError();

            HttpException httpException = exception as HttpException;

            // Obtém o código HTTP da exceção, ou 500 se não for uma HttpException
            int code = httpException?.GetHttpCode() ?? 500;

            Response.Redirect(url: $"/ErrorHandler/Index?code={code}");
        }

        // Configura a autorização de usuários e roles após o usuário ser autenticado pela aplicação.
        // https://www.youtube.com/watch?v=F4V2Olx6ydU
        protected void Application_PostAuthenticateRequest(object sender, EventArgs e)
        {
            HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];

            if (authCookie != null)
            {
                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);

                if (authTicket != null && !authTicket.Expired)
                {
                    string[] roles = authTicket.UserData.Split(',');

                    GenericPrincipal userPrincipal = new GenericPrincipal(new GenericIdentity(authTicket.Name), roles);

                    HttpContext.Current.User = userPrincipal;
                }
            }
        }
    }
}