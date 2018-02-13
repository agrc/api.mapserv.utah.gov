using System.Collections.Generic;
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
using WebAPI.Domain;
using WebAPI.Domain.ApiResponses;
using WebAPI.Domain.ArcServerResponse.Geolocator;

namespace WebAPI.API.Commands.Geocode
{
    public class ReverseGeocodeCommand : Command<ReverseGeocodeResult>
    {
        public ReverseGeocodeCommand(Location location, int wkid, double distance)
        {
            Location = location;
            Wkid = wkid;
            Distance = distance;
        }

        public Location Location { get; set; }
        public LocatorDetails LocatorInfo { get; set; }
        public int Wkid { get; set; }
        public double Distance { get; set; }

        public override string ToString()
        {
            return $"ReverseGeocodeCommand, Location: {Location}, Wkid: {Wkid}, Distance: {Distance}";
        }

        protected override void Execute()
        {
            if (Wkid != 26912)
            {
                var reprojectPointCommand =
                    new ReprojectPointsCommand(new ReprojectPointsCommand.PointProjectQueryArgs(Wkid, 26912,
                                                                                                new List<double>
                                                                                                    {
                                                                                                        Location.X,
                                                                                                        Location.Y
                                                                                                    }));

                var pointReprojectResponse = CommandExecutor.ExecuteCommand(reprojectPointCommand);

                if (!pointReprojectResponse.IsSuccessful || !pointReprojectResponse.Geometries.Any())
                    return;

                var points = pointReprojectResponse.Geometries.FirstOrDefault();

                if (points != null)
                {
                    Location = new Location(points.X, points.Y);
                }
            }

            var locator = CommandExecutor.ExecuteCommand(new GetAltNameLocatorsForLocationCommand(Location));

            var requestUri = string.Format(locator.Url, Location.X, Location.Y, Distance, Wkid);

            var response =
                App.HttpClient.GetAsync(requestUri).ContinueWith(
                    httpResponse => ConvertResponseToObjectAsync(httpResponse.Result)).Unwrap().Result;

            Result = Mapper.Map<ReverseGeocodeResponse, ReverseGeocodeResult>(response);
        }

        private static Task<ReverseGeocodeResponse> ConvertResponseToObjectAsync(HttpResponseMessage task)
        {
            return task.Content.ReadAsAsync<ReverseGeocodeResponse>(new MediaTypeFormatter[]
                {
                    new TextPlainResponseFormatter()
                });
        }
    }
}