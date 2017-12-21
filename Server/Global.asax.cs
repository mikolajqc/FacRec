﻿using Commons.Inferfaces.DAOs;
using FaceRecognition.Services;
using Server.DAO;
using SimpleInjector;
using SimpleInjector.Integration.WebApi;
using SimpleInjector.Lifestyles;
using System.Web.Http;
using Commons.Inferfaces.Services;
using FisherFaceRecognition.Services;

namespace Server
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        { 
            //SimpleInjector configuration
            var container = new Container();
            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            //Services
            container.Register<IRecognitonService, FisherFacesRecognitionService>(Lifestyle.Transient);
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
