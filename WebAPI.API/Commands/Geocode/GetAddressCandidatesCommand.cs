using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AutoFixture;
using WebAPI.API.Exceptions;
using WebAPI.Common.Abstractions;
using WebAPI.Common.Formatters;
using WebAPI.Domain;
using WebAPI.Domain.ArcServerResponse.Geolocator;
using Serilog;

namespace WebAPI.API.Commands.Geocode
{
    public class GetAddressCandidatesCommand : Command<Task<List<Candidate>>>
    {
        private HttpClient _httpClient;

        public GetAddressCandidatesCommand(LocatorDetails input)
        {
            LocatorDetails = input;
        }

        public GetAddressCandidatesCommand(LocatorDetails input, bool testing)
        {
            LocatorDetails = input;
            Testing = testing;
        }

        internal LocatorDetails LocatorDetails { get; set; }
        private bool Testing { get; set; }

        protected void Initialize()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
        }

        protected override void Execute()
        {
            if (Testing)
            {
                var fixture = new Fixture();
                var list = fixture.CreateMany<Candidate>(10).ToList();

                Result = Task.Factory.StartNew(() => list);

                return;
            }

            Initialize();

            Task<List<Candidate>> result;

            try
            {
                Log.Debug("Request sent to locator, url={Url}", LocatorDetails.Url);
                result = _httpClient.GetAsync(LocatorDetails.Url).ContinueWith(
                    httpResponse =>
                    ConvertResponseToObjectAsync(httpResponse.Result).ContinueWith(model => ProcessResult(model.Result)).
                                                                      Unwrap()).Unwrap();
            }
            catch (AggregateException ex)
            {
                Log.Fatal(ex, "Error requesting address candidates. {message}", ex.Flatten().Message);
                result = null;
                ErrorMessage = ex.Message;
            }

            Result = result;
        }

        protected Task<List<Candidate>> ProcessResult(GeocodeAddressResponse task)
        {
            return Task.Factory.StartNew(() =>
                {
                    if (task.Error != null && task.Error.Code == 400)
                    {
                        throw new GeocodingException($"{LocatorDetails.Name} geocoder is not started.");
                    }

                    var result = task.Candidates;

                    if (result == null)
                        return result;

                    result.ForEach(x =>
                        {
                            x.Locator = LocatorDetails.Name;
                            x.Weight = LocatorDetails.Weight;
                            x.AddressGrid = ParseAddressGrid(x.Address);
                        });

                    return result;
                });
        }

        private static string ParseAddressGrid(string address)
        {
            if (!address.Contains(","))
                return null;

            var addressParts = address.Split(',');

            return addressParts[1].Trim();
        }

        private static Task<GeocodeAddressResponse> ConvertResponseToObjectAsync(HttpResponseMessage task)
        {
            Task<GeocodeAddressResponse> response;

            try
            {
                response = task.Content.ReadAsAsync<GeocodeAddressResponse>(new MediaTypeFormatter[]
                {
                    new TextPlainResponseFormatter()
                });
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Error reading geocode address response {Response}", task.Content.ReadAsStringAsync().Result);
                throw;
            }

            return response;
        }

        public override string ToString()
        {
            return $"GetAddressCandidatesCommand, LocatorDetails: {LocatorDetails}";
        }
    }
}