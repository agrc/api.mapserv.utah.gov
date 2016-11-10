using System.Web.Http;
using System.Web.Http.Dispatcher;
using SDammann.WebApi.Versioning;
using WebApiContrib.Formatting.Jsonp;

namespace WebAPI.API
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
#if DEBUG
            config.EnableSystemDiagnosticsTracing();
#endif

            config.Services.Replace(typeof (IHttpControllerSelector),
                                    new RouteVersionedControllerSelector(
                                        GlobalConfiguration.Configuration));

            config.AddJsonpFormatter(callbackQueryParameter:"callback");

            //http://www.strathweb.com/2013/06/supporting-only-json-in-asp-net-web-api-the-right-way/
            //config.Services.Replace(typeof(IContentNegotiator), new JsonContentNegotiator());

            /*
                ==Geocoding Endpoints==
            */

            config.Routes.MapHttpRoute(
                name: "v1_GeocodeApi",
                routeTemplate: "api/v{version}/geocode/{street}/{zone}",
                defaults: new
                {
                    action = "get",
                    controller = "Geocode",
                    version = "1"
                });

            config.Routes.MapHttpRoute(
                name: "v1_GeocodeMultipleApi",
                routeTemplate: "api/v{version}/geocode/multiple",
                defaults: new
                {
                    action = "multiple",
                    controller = "Geocode",
                    version = "1"
                }); 
            
            config.Routes.MapHttpRoute(
                name: "v1_GeocodeReverseApi",
                routeTemplate: "api/v{version}/geocode/reverse/{x}/{y}",
                defaults: new
                {
                    action = "reverse",
                    controller = "Geocode",
                    version = "1"
                });

            config.Routes.MapHttpRoute(
               name: "v1_GeocodeRouteMilePost",
               routeTemplate: "api/v{version}/geocode/milepost/{route}/{milepost}",
               defaults: new
               {
                   action = "RouteMilePost",
                   controller = "Geocode",
                   version = "1"
               });

            config.Routes.MapHttpRoute(
               name: "v1_GeocodeReverseMilePost",
               routeTemplate: "api/v{version}/geocode/reversemilepost/{x}/{y}",
               defaults: new
               {
                   action = "ReverseMilePost",
                   controller = "Geocode",
                   version = "1"
               });

            config.Routes.MapHttpRoute(
               name: "v1_AgoGeocodeActivationApi",
               routeTemplate: "api/v{version}/geocode/ago/agrc-ago/GeocodeServer",
               defaults: new
               {
                   action = "ArcGisOnlineActivation",
                   controller = "Geocode",
                   version = "1"
               });

            config.Routes.MapHttpRoute(
              name: "v1_AgoGeocodeApi",
              routeTemplate: "api/v{version}/geocode/ago/agrc-ago/GeocodeServer/findAddressCandidates",
              defaults: new
              {
                  action = "ArcGisOnline",
                  controller = "Geocode",
                  version = "1"
              });

            /*
                ==Search Endpoints==
            */

            //http://webapi/api/v1/search/sgid10.boundaries.counties/name?apiKey=
            config.Routes.MapHttpRoute(
                name: "v1_SearchApi",
                routeTemplate: "api/v{version}/search/{featureClass}/{returnValues}",
                defaults: new
                {
                    action = "get",
                    controller = "Search",
                    version = "1"
                });

            /*
                ==Info Endpoints==
            */

            config.Routes.MapHttpRoute(
                name: "v1_InfoApi",
                routeTemplate: "api/v{version}/info/FeatureClassNames",
                defaults: new
                {
                    action = "FeatureClassNames",
                    controller = "Info",
                    version = "1"
                });

            config.Routes.MapHttpRoute(
               name: "v1_InfoAttributesApi",
               routeTemplate: "api/v{version}/info/FieldNames/{featureClass}",
               defaults: new
               {
                   action = "FeatureClassAttributes",
                   controller = "Info",
                   version = "1"
               });
        }
    }
}