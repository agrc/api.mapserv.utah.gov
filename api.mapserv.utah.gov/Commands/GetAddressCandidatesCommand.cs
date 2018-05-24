using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using api.mapserv.utah.gov.Exceptions;
using api.mapserv.utah.gov.Extensions;
using api.mapserv.utah.gov.Models;

namespace api.mapserv.utah.gov.Commands
{
    public class GetAddressCandidatesCommand
    {
        private readonly HttpClient _client;

        public GetAddressCandidatesCommand(HttpClient client)
        {
            _client = client;
        }

        internal LocatorProperties LocatorDetails { get; set; }

        public void Initialize(LocatorProperties input)
        {
            LocatorDetails = input;
        }

        public async Task<IEnumerable<Candidate>> Execute()
        {
//            Log.Debug("Request sent to locator, url={Url}", LocatorDetails.Url);
// TODO create a polly policy for the locators
            var httpResponse = await _client.GetAsync(LocatorDetails.Url).ConfigureAwait(false);

            try
            {
                var geocodeResponse = await httpResponse.Content.ReadAsAsync<LocatorResponse>().ConfigureAwait(false);

                return ProcessResult(geocodeResponse);
            }
            catch (Exception ex)
            {
                //                Log.Fatal(ex, "Error reading geocode address response {Response}",
                //                          response.Content.ReadAsStringAsync().Result);
                throw;
            }
        }

        protected IEnumerable<Candidate> ProcessResult(LocatorResponse response)
        {
            if (response.Error != null && response.Error.Code == 500)
            {
//                    Log.Fatal($"{LocatorDetails.Name} geocoder is not started.");

                throw new GeocodingException($"{LocatorDetails.Name} geocoder is not started.");
            }

            var result = response.Candidates;

            if (result == null)
            {
                return null;
            }

            foreach (var candidate in result)
            {
                candidate.Locator = LocatorDetails.Name;
                candidate.Weight = LocatorDetails.Weight;
                candidate.AddressGrid = ParseAddressGrid(candidate.Address);
            }

            return result;
        }

        private static string ParseAddressGrid(string address)
        {
            if (!address.Contains(","))
            {
                return null;
            }

            var addressParts = address.Split(',');

            return addressParts[1].Trim();
        }

        public override string ToString() => $"GetAddressCandidatesCommand, LocatorDetails: {LocatorDetails}";
    }
}
