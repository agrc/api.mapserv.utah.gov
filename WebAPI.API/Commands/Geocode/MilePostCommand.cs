using System;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using WebAPI.Common.Abstractions;
using WebAPI.Common.Formatters;
using WebAPI.Common.Models.Esri.RoadsAndHighways;
using WebAPI.Domain.ApiResponses;
using WebAPI.Domain.ArcServerResponse.Geolocator;
using WebAPI.Domain.InputOptions;

namespace WebAPI.API.Commands.Geocode
{
    public class MilepostCommand : AsyncCommand<RouteMilepostResult>
    {
        private const string BaseUrl = "https://maps.udot.utah.gov/randh/rest/services/ALRS/MapServer/exts/LRSServer/networkLayers/0/";

        public MilepostCommand(string route, double milepost, MilepostOptions options)
        {
            Route = route;
            Milepost = milepost;
            Options = options;
        }

        public string Route { get; set; }
        public double Milepost { get; set; }
        public MilepostOptions Options { get; set; }

        public override string ToString()
        {
            return $"MilepostCommand, Route: {Route}, Milepost: {Milepost}, Options: {Options}";
        }

        public override async Task<RouteMilepostResult> Execute()
        {
            var requestContract = new MeasureToGeometry.RequestContract
            {
                Locations = new[] {
                        new MeasureToGeometry.RequestLocation {
                            Measure = Milepost.ToString(),
                            RouteId = Route
                        }
                    },
                OutSr = Options.WkId
            };

            var requestUri = $"{BaseUrl}measureToGeometry{requestContract.QueryString}";

            HttpResponseMessage httpResponse;

            try
            {
                httpResponse = await App.HttpClient.GetAsync(requestUri);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;

                return null;
            }

            MeasureToGeometry.ResponseContract response;
            try
            {
                response = await httpResponse.Content.ReadAsAsync<MeasureToGeometry.ResponseContract>(new MediaTypeFormatter[]
                {
                    new TextPlainResponseFormatter()
                });
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;

                return null;
            }

            var result = ProcessResult(response);
             
            if (result is null)
            {
                return null;
            }

            result.InputRouteMilePost = $"Route {Route} Milepost {Milepost}";

            return result;
        }

        private RouteMilepostResult ProcessResult(MeasureToGeometry.ResponseContract response)
        {
            if (response.Locations?.Length != 1)
            {
                // we have a problem
            }

            var location = response.Locations[0];

            if (location.Status != MeasureToGeometry.Status.esriLocatingOK)
            {
                // we have a problem

                // TODO: create messages from status
                return null;
            }

            if (location.GeometryType != GeometryType.esriGeometryPoint)
            {
                // we have another problem
            }

            return new RouteMilepostResult
            {
                Source = "UDOT Roads and Highways",
                Location = new Location(location.Geometry.X, location.Geometry.Y),
                MatchRoute = $"Route {location.RouteId}, Milepost {location.Geometry.M}"
            };
        }
    }
}