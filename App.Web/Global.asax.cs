using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Http;
using System.Security.Claims;
using System.Web.Helpers;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using System.Reflection;
using App.Core;
using App.WebApi;
using System.Web.Optimization;

namespace App.Web
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            //HibernatingRhinos.Profiler.Appender.EntityFramework.EntityFrameworkProfiler.Initialize();

            // 在应用程序启动时运行的代码
            AreaRegistration.RegisterAllAreas();

            RegisterService();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RegisterRoutes(RouteTable.Routes);

            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.Name;
        }

        private void RegisterService()
        {
            var builder = new ContainerBuilder();
            string ControllerDllNS = "App.Web";  //Controller程序集命名空间
            string ApiControllerDllNS = "App.WebApi";  //Controller程序集命名空间
            string BusinessDllNS = "App.Core";      //业务逻辑程序集命名空间
            var controllerDll = Assembly.Load(ControllerDllNS);
            var apiControllerDll = Assembly.Load(ApiControllerDllNS);
            var businessDll = Assembly.Load(BusinessDllNS);

            builder.RegisterControllers(controllerDll);
            builder.RegisterApiControllers(apiControllerDll);

            builder.RegisterAssemblyTypes(controllerDll).AsImplementedInterfaces();
            builder.RegisterAssemblyTypes(businessDll).AsImplementedInterfaces();

            var container = builder.Build();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }

        private void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "App.Web.Controllers" }
            );
        }
    }
}