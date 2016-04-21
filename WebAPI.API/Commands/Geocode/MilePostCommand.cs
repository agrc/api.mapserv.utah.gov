using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AutoMapper;
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

        private HttpClient _httpClient;

        public MilepostCommand(string route, double milepost, MilepostOptions options)
        {
            Route = route;
            Milepost = milepost;
            Options = options;
        }

        public string Route { get; set; }
        public double Milepost { get; set; }
        public MilepostOptions Options { get; set; }

        protected void Initialize()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
        }

        public override string ToString()
        {
            return string.Format("{0}, Route: {1}, Milepost: {2}, Options: {3}", "MilepostCommand", Route, Milepost,
                                 Options);
        }

        protected override void Execute()
        {
            Initialize();
            var requestUri = string.Format(Url, Route, Milepost, Options.Side);

            var response = _httpClient.GetAsync(requestUri).ContinueWith(
                httpResponse => ConvertResponseToObjectAsync(httpResponse.Result)).Unwrap().Result;

            var result = Mapper.Map<GeocodeMilepostResponse, RouteMilepostResult>(response);
            result.InputRouteMilePost = string.Format("Route {0} Milepost {1}", Route, Milepost);

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