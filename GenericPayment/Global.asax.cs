using System;
using System.Net;
using System.Web.Mvc;
using System.Web.Routing;

namespace GenericPayment
{
    public class GenericPaymentApplication : System.Web.HttpApplication
    {
        private static string _databasePath;
        public static string DatabasePath { get { return _databasePath; } }

        protected void Application_Start(object sender, EventArgs e)
        {
            _databasePath = Server.MapPath("~/Database");

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}