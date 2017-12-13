using Commons.Inferfaces.DAOs;
using FaceRecognition.Interfaces;
using FaceRecognition.Services;
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
            container.Register<IRecognitonService, RecognitionService>(Lifestyle.Transient);
            container.Register<IAddNewFaceService, AddNewFaceService>(Lifestyle.Transient);
            container.Register<ILearningService, LearningService>(Lifestyle.Transient);

            //DAOs
            container.Register<IAverageVectorDao, AverageVectorDao>(Lifestyle.Transient);
            container.Register<IEigenFaceDao, EigenFaceDao>(Lifestyle.Transient);
            container.Register<IWageDao, WageDao>(Lifestyle.Transient);

            container.RegisterWebApiControllers(GlobalConfiguration.Configuration);
            container.Verify();
            GlobalConfiguration.Configuration.DependencyResolver =
                new SimpleInjectorWebApiDependencyResolver(container);

            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
