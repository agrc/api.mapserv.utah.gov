using System;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using api.mapserv.utah.gov.Formatters;
using api.mapserv.utah.gov.Models;
using api.mapserv.utah.gov.Models.ArcGis;
using Serilog;

namespace api.mapserv.utah.gov.Commands
{
    public class ReverseGeocodeAddressCommand
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly MediaTypeFormatter[] _mediaTypes;

        public ReverseGeocodeAddressCommand(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
            _mediaTypes = new MediaTypeFormatter[]
            {
                new TextPlainResponseFormatter()
            };
        }

        internal LocatorProperties LocatorDetails { get; set; }

        public void Initialize(LocatorProperties input)
        {
            LocatorDetails = input;
        }

        public async Task<ReverseGeocodeRestResponse> Execute()
        {
            Log.Debug("Request sent to locator, url={Url}", LocatorDetails.Url);

            // TODO create a polly policy for the locators
            var client = _clientFactory.CreateClient("default");
            var httpResponse = await client.GetAsync(LocatorDetails.Url).ConfigureAwait(false);

            try
            {
                var reverseResponse = await httpResponse.Content.ReadAsAsync<ReverseGeocodeRestResponse>(_mediaTypes).ConfigureAwait(false);

                if (!reverseResponse.IsSuccessful)
                {
                    return null;
                }

                return reverseResponse;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Error reading geocode address response {Response} from {locator}",
                          await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false), LocatorDetails);
                throw;
            }
        }
    }
}
