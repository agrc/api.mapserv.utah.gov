using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using WebAPI.Common.Abstractions;
using WebAPI.Common.Commands.Spatial;
using WebAPI.Common.Executors;
using WebAPI.Common.Formatters;
using WebAPI.Domain.ApiResponses;
using WebAPI.Domain.ArcServerResponse.Geolocator;
using WebAPI.Domain.ArcServerResponse.Soe;
using WebAPI.Domain.InputOptions;

namespace WebAPI.API.Commands.Geocode
{
    public class MilepostCommand : Command<RouteMilepostResult>
    {
        private static readonly string Url = ConfigurationManager.AppSettings["milepost_url"];

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

        protected override void Execute()
        {
            var requestUri = string.Format(Url, Route, Milepost, Options.Side, Options.FullRoute);

            var response = App.HttpClient.GetAsync(requestUri).ContinueWith(
                httpResponse => ConvertResponseToObjectAsync(httpResponse.Result)).Unwrap().Result;

            var result = new RouteMilepostResult { 
                Source = response.Geocoder,
                MatchRoute = response.MatchAddress,
                Location = new Location
                {
                    X = response.UTM_X,
                    Y = response.UTM_Y
                }
            };

            result.InputRouteMilePost = $"Route {Route} Milepost {Milepost}";

            if (Options.WkId != 26912)
            {
                var reprojectPointCommand =
                    new ReprojectPointsCommand(new ReprojectPointsCommand.PointProjectQueryArgs(26912, Options.WkId,
                                                                                                new List<double>
                                                                                                    {
                                                                                                        result.Location.X,
                                                                                                        result.Location.Y
                                                                                                    }));

                var pointReprojectResponse = CommandExecutor.ExecuteCommand(reprojectPointCommand);

                if (!pointReprojectResponse.IsSuccessful || !pointReprojectResponse.Geometries.Any())
                    return;

                var points = pointReprojectResponse.Geometries.FirstOrDefault();

                if (points != null)
                {
                    result.Location = new Location(points.X, points.Y);
                }
            }

            Result = result;
        }

        private static Task<GeocodeMilepostResponse> ConvertResponseToObjectAsync(HttpResponseMessage task)
        {
            return task.Content.ReadAsAsync<GeocodeMilepostResponse>(new MediaTypeFormatter[]
                {
                    new TextPlainResponseFormatter()
                });
        }
    }
}