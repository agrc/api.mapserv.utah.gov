using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AutoMapper;
using WebAPI.Common.Abstractions;
using WebAPI.Common.Executors;
using WebAPI.Common.Formatters;
using WebAPI.Domain;
using WebAPI.Domain.ApiResponses;
using WebAPI.Domain.ArcServerResponse.Geolocator;

namespace WebAPI.API.Commands.Geocode
{
    public class ReverseGeocodeCommand : Command<ReverseGeocodeResult>
    {
        private HttpClient _httpClient;

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

        protected void Initialize()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
        }

        public override string ToString()
        {
            return string.Format("{0}, Location: {1}, Wkid: {2}, Distance: {3}", "ReverseGeocodeCommand", Location, Wkid,
                                 Distance);
        }

        protected override void Execute()
        {
            Initialize();

            var locator = CommandExecutor.ExecuteCommand(new GetLocatorsForLocationCommand(Location));

            var requestUri = string.Format(locator.Url, Location.X, Location.Y, Distance, Wkid);

            var response =
                _httpClient.GetAsync(requestUri).ContinueWith(
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