using Ninject;
using Ninject.Web.Common;
using WebAPI.API.Handlers.Delegating;
using WebAPI.API.Ninject.Modules;
using WebAPI.Common.Ninject.Modules;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(WebAPI.API.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(WebAPI.API.App_Start.NinjectWebCommon), "Stop")]

namespace WebAPI.API.App_Start
{
    using System;
    using System.Web;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    public static class NinjectWebCommon 
    {
        private static readonly Bootstrapper Bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start() 
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            Bootstrapper.Initialize(CreateKernel);
        }
        
        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            Bootstrapper.ShutDown();
        }
        
        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            App.Kernel = new StandardKernel();
            try
            {
                App.Kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                App.Kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                RegisterServices(App.Kernel);
                return App.Kernel;
            }
            catch
            {
                App.Kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            kernel.Load(typeof(RavenModule).Assembly);
            kernel.Load(typeof(DomainLinkModule).Assembly);
//            kernel.Bind<AuthorizeRequestHandler>().To<AuthorizeRequestHandler>();
        }        
    }
}
