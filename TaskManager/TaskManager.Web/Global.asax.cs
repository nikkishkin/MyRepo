using System;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using TaskManager.Core;
using TaskManager.Web.Configuration;
using TaskManager.Web.Controllers;
using DependencyResolver = TaskManager.Core.DependencyResolving.DependencyResolver;

namespace TaskManager.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            DependencyConfig.Configure();
        }

        protected void Application_BeginRequest()
        {
            IUnitOfWork unitOfWork = DependencyResolver.Resolve<IUnitOfWork>();

            HttpContext.Current.Items.Add(TaskManagerController.REQUEST_STORAGE_UNIT_OF_WORK, unitOfWork);
        }

        protected void Application_EndRequest()
        {
            IUnitOfWork unitOfWork = (IUnitOfWork)HttpContext.Current.Items[TaskManagerController.REQUEST_STORAGE_UNIT_OF_WORK];

            unitOfWork.Dispose();
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        {
            if (Request.IsAuthenticated)
            {
                HttpCookie httpCookie = Context.Request.Cookies[FormsAuthentication.FormsCookieName];

                if (httpCookie != null)
                {
                    FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(httpCookie.Value);

                    UserPrincipal userPrincipal = (UserPrincipal) Application[ticket.UserData];
                    if (userPrincipal != null)
                    {
                        UserPrincipal.CurrentPrincipal = userPrincipal;
                        HttpContext.Current.User = userPrincipal;
                        return;
                    }
                }
            }

            UserPrincipal.CurrentPrincipal = UserPrincipal.Empty;
            HttpContext.Current.User = UserPrincipal.Empty;

            //HttpCookie authCookie = Request.Cookies["authCookie"];

            //if (authCookie != null)
            //    Thread.CurrentPrincipal =
            //        new GenericPrincipal(new GenericIdentity(authCookie.Value, "Passport"), null);
        }
    }
}