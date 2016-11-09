using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Ninject;
using WebAPI.Dashboard.Cache;

namespace WebAPI.Dashboard
{
    public class App : HttpApplication
    {
        public static IKernel Kernel { get; set; }
        public const int KeySoftLimit = 50;
        public static readonly string Pepper = "B5tNpCC9";
        public static string NotifyList = "sgourley@utah.gov";
        internal static HashSet<string> ConnectedClients = new HashSet<string>();

        [Inject]
        public static OutputCache OutputCache { get; set; }

        internal static string OutputCacheKey
        {
            get
            {
                if (HttpContext.Current.Session["App.OutputCacheKey"] == null)
                {
                    ResetOutputCache();
                }

                var key = HttpContext.Current.Session["App.OutputCacheKey"];
               
                return key != null ? key.ToString() : "";
            }
        }

        internal static void ResetOutputCache()
        {
            if (OutputCache == null)
                OutputCache = Kernel.Get<OutputCache>();

            OutputCache.Reset("App.OutputCacheKey", Guid.NewGuid().ToString());
        }

        protected void Application_Start()
        {
            ViewEngineConfig.RegisterViewEngine(new RazorViewEngine());
            AreaRegistration.RegisterAllAreas();

            //FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AutoMapperConfig.RegisterMaps();
        }

        public override string GetVaryByCustomString(HttpContext context, string custom)
        {
            if (custom == "App.User.Identity.Name")
            {
                return context.User.Identity.IsAuthenticated
                           ? context.User.Identity.Name
                           : "";
            }

            return custom == "App.OutputCacheKey" ? OutputCacheKey : base.GetVaryByCustomString(context, custom);
        }
    }
}