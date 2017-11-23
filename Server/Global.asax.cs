using FaceRecognition;
using SimpleInjector;
using SimpleInjector.Integration.WebApi;
using SimpleInjector.Lifestyles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace Server
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        { 
            ///SimpleInjector configuration
            var container = new Container();
            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
            container.Register<IFaceRecognition, FaceRecognition.FaceRecognition>(Lifestyle.Scoped);
            container.RegisterWebApiControllers(GlobalConfiguration.Configuration);
            container.Verify();
            GlobalConfiguration.Configuration.DependencyResolver =
                new SimpleInjectorWebApiDependencyResolver(container);

            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
