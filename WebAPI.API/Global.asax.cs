using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Ninject;
using WebAPI.Common.Logging;
using WebAPI.Domain;
using WebAPI.Domain.Linkers;


namespace WebAPI.API
{
    /// <summary>
    /// Main Application Class
    /// </summary>
    public class App : HttpApplication
    {
        public static IKernel Kernel { get; set; }

        /// <summary>
        ///     Gets or sets the sgid categories.
        /// </summary>
        /// <value>
        ///     The sgid categories.
        /// </value>
        public static IEnumerable<string> SgidCategories { get; set; }

        /// <summary>
        ///     Gets or sets the utah street type abbreviations.
        /// </summary>
        /// <value>
        ///     The abbreviations.
        /// </value>
        public static Dictionary<StreetType, string> StreetTypeAbbreviations { get; set; }

        /// <summary>
        ///     Gets or sets the zip codes that have address points.
        /// </summary>
        /// <value>
        ///     The zip codes with address points.
        /// </value>
        public static IEnumerable<string> GridsWithAddressPoints { get; set; }

        /// <summary>
        /// Gets or sets the zip code to address grid lookup.
        /// </summary>
        /// <value>
        /// The zip code to address grid lookup.
        /// </value>
        public static Dictionary<string, List<GridLinkable>> ZipCodeGridLookup { get; set; }

        /// <summary>
        /// Gets or sets the city grid lookup.
        /// </summary>
        /// <value>
        /// The city grid lookup.
        /// </value>
        public static Dictionary<string, List<GridLinkable>> PlaceGridLookup { get; set; }

        public static Dictionary<string,List<GridLinkable>> UspsDeliveryPoints { get; set; }
        
        public static Dictionary<string, Regex> RegularExpressions { get; set; }

        /// <summary>
        /// Gets or sets the unit abbreviations.
        /// http://pe.usps.gov/cpim/ftp/pubs/pub28/pub28.pdf page 76
        /// </summary>
        /// <value>
        /// the full name
        /// The unit abbreviations for common secondary units.
        /// if the unit needs a number after it
        /// </value>
        public static List<Tuple<string, string, bool>> UnitAbbreviations { get; set; }

        public static Dictionary<int, PoBoxAddress> PoBoxLookup { get; set; }

        public static IEnumerable<int> PoBoxZipCodesWithExclusions { get; set; }

        public static Dictionary<int, PoBoxAddressCorrection> PoBoxExclusions { get; set; }

        /// <summary>
        ///     Runs when the application starts or the app pool recycles.
        /// </summary>
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            LoggingConfig.Register("api");
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            ModelBindingConfig.RegisterModelBindings(GlobalConfiguration.Configuration.Services);
            ViewEngineConfig.RegisterViewEngine(new RazorViewEngine());
            HandlerConfig.RegisterHandlers(GlobalConfiguration.Configuration.MessageHandlers);
            AutoMapperConfig.RegisterMaps();
            CacheConfig.BuildCache();
            FormatterConfig.RegisterFormatters(GlobalConfiguration.Configuration.Formatters);
        }
    }
}