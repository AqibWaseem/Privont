using Microsoft.Owin.Security.Google;
using Microsoft.Owin;
using Owin;
using Privont.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.Owin.Security.Cookies;
using Microsoft.AspNet.Identity;
using System.Security.Claims;
using System.Web.Helpers;

namespace Privont
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //AppDomain.CurrentDomain.SetData("owin:AppStartup", typeof(StartupConfigGoogleAuth).AssemblyQualifiedName);
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            General.ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();
        }
       
    }
}
