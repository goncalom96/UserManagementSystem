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

            // Limpa a última exceção da lista de erros do servidor
            Server.ClearError();

            // Tenta converter a exceção em HttpException
            HttpException httpException = exception as HttpException;

            // Obtém o código HTTP da exceção, ou 500 se não for uma HttpException
            int code = httpException?.GetHttpCode() ?? 500;

            // Redireciona o usuário para a página de tratamento de erros com o código apropriado
            Response.Redirect(url: $"/ErrorHandler/Index?code={code}");
        }

        // Configura a autorização de usuários e roles após o usuário ser autenticado pela aplicação.
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