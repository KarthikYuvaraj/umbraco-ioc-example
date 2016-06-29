using Castle.MicroKernel.Registration;
using Castle.Windsor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;
using Umbraco.Core;
using Umbraco.Web;
using Umbraco.Web.Mvc;
using WebApplication12.Controllers;

namespace WebApplication12
{
    public class CustomUmbracoApplication : UmbracoApplication
    {
        protected override IBootManager GetBootManager()
        {
            return new CustomBootManager(this);
        }
    }

    class CustomBootManager : WebBootManager
    {
        public CustomBootManager(UmbracoApplicationBase umbracoApplication) : base(umbracoApplication)
        {
        }

        protected override void InitializeResolvers()
        {
            base.InitializeResolvers();

            FilteredControllerFactoriesResolver.Current.InsertType<CustomFilteredControllerFactory>(0);
        }

        public override IBootManager Complete(Action<ApplicationContext> afterComplete)
        {
            RouteTable.Routes.MapRoute(
                "ProductHome",
                "product/index",
                new { controller = "Product", action = "index" }
            );

            return base.Complete(afterComplete);
        }
    }

    class CustomFilteredControllerFactory : IFilteredControllerFactory
    {
        private WindsorContainer container;

        public CustomFilteredControllerFactory()
        {
            container = new WindsorContainer();
            container.Register(
                Classes.FromAssembly(GetType().Assembly)
                    .BasedOn<IController>()
                    .Configure(c => c.Named(c.Implementation.Name.Replace("Controller", "")))
                    .WithServiceAllInterfaces()
                    .LifestylePerWebRequest(),
                Classes.FromAssembly(GetType().Assembly)
                    .Where(t => t.Name.EndsWith("Service"))
            );
        }
        public bool CanHandle(RequestContext request)
        {
            return (string)request.RouteData.Values["controller"] != "RenderMvc";
        }

        public IController CreateController(RequestContext requestContext, string controllerName)
        {
            return container.Resolve<IController>(controllerName);
        }

        public SessionStateBehavior GetControllerSessionBehavior(RequestContext requestContext, string controllerName)
        {
            return SessionStateBehavior.Disabled;
        }

        public void ReleaseController(IController controller)
        {
        }
    }
}