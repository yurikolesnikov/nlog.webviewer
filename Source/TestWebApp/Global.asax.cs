using System;
using NLog;

namespace TestWebApp
{
    public class Global : System.Web.HttpApplication
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        protected void Application_Start(object sender, EventArgs e)
        {
            _log.Info("Application Start");
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            _log.Info("Session Start");
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            _log.Info("Application BeginRequest");
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            _log.Info("Application AuthenticateRequest");
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            _log.Error("Application Error");
        }

        protected void Session_End(object sender, EventArgs e)
        {
            _log.Info("Session End");
        }

        protected void Application_End(object sender, EventArgs e)
        {
            _log.Info("Application End");
        }
    }
}