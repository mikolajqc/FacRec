using Commons.Inferfaces.DAOs;
using FaceRecognition;
using FaceRecognition.Interfaces;
using Server.DAO;
using SimpleInjector;
using SimpleInjector.Integration.WebApi;
using SimpleInjector.Lifestyles;
using System.Web.Http;

namespace Server
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        { 
            ///SimpleInjector configuration
            var container = new Container();
            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            //Services
            container.Register<IFaceRecognition, FaceRecognition.FaceRecognition>(Lifestyle.Transient);
            container.Register<IRecognitonService, RecognitionService>(Lifestyle.Transient);

            //DAOs
            container.Register<IAverageVectorDAO, AverageVectorDAO>(Lifestyle.Transient);
            container.Register<IEigenFaceDAO, EigenFaceDAO>(Lifestyle.Transient);
            container.Register<IWageDAO, WageDAO>(Lifestyle.Transient);

            container.RegisterWebApiControllers(GlobalConfiguration.Configuration);
            container.Verify();
            GlobalConfiguration.Configuration.DependencyResolver =
                new SimpleInjectorWebApiDependencyResolver(container);

            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
