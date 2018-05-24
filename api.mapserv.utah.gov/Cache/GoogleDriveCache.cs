using System;
using System.Collections.Generic;
using api.mapserv.utah.gov.Models;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Polly;
using Polly.Retry;

namespace api.mapserv.utah.gov.Cache
{
    public class GoogleDriveCache : IGoogleDriveCache
    {
        private const string ApplicationName = "Geocoding Cache";
        private static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
        private readonly RetryPolicy _retryPolicy;

        public Dictionary<string, List<GridLinkable>> PlaceGrids { get; set; }

        public Dictionary<string, List<GridLinkable>> ZipCodesGrids { get; set; }

        public Dictionary<string, List<GridLinkable>> UspsDeliveryPoints { get; set; }

        public IEnumerable<PoBoxAddressCorrection> PoBoxExclusions { get; set; }

        public Dictionary<int, PoBoxAddress> PoBoxes { get; set; }

        public GoogleDriveCache(GoogleCredential creds)
        {
            var credential = creds.CreateScoped(Scopes);

            _retryPolicy = Policy.Handle<Exception>()
                                 .WaitAndRetryForever(retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            using (var service = new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
                GZipEnabled = true
            }))
            {
                service.HttpClient.Timeout = TimeSpan.FromSeconds(30);

                // TODO make async
                var places = GetCityPlaceNames(service, "1Dc77HLVn9YXgY1JkcqqaMh1Tm0cemwXxFfyHb7dP0Cs", "A2:C");
                var zips = GetZipCodesInQuadrants(service, "1HX47abPQ24LqB8iUHu6HTSmTnbU1YN9WvxO6BDsHCLg", "A2:C");
                var deliveryPoints = GetUspsDeliveryPoints(service, "1rgqNweBxiqWVwxTlMWgX-sN8IJguvlUcY0yOvmI8JuQ", "A2:H");
                var corrections = GetCorrections(service, "1ZqXyQflKQ8q-sBHCHLaYaQXY90aOFU0JPc4-M_41GM0", "A2:H");
                var poboxes = GetPoBoxes(service, "1DX5w1UDeANyrjr0C-13lJVal2sRcZ3U67Im1loIaFog", "A2:C");

                PlaceGrids = BuildGridLinkableLookup(places);
                ZipCodesGrids = BuildGridLinkableLookup(zips);
                UspsDeliveryPoints = BuildGridLinkableLookup(deliveryPoints);
                PoBoxExclusions = corrections;
                PoBoxes = poboxes;
            }
        }

        private Dictionary<int, PoBoxAddress> GetPoBoxes(SheetsService service, string spreadsheet, string range)
        {
            var response = _retryPolicy.Execute(() => service.Spreadsheets.Values.Get(spreadsheet, range).Execute());
            var values = response.Values;

            var items = new Dictionary<int, PoBoxAddress>();

            if (values == null)
            {
                return items;
            }

            foreach (var row in values)
            {
                items.Add(Convert.ToInt32(row[0]), new PoBoxAddress(Convert.ToInt32(row[0]), Convert.ToDouble(row[1]), Convert.ToDouble(row[2])));
            }

            return items;
        }

        private IEnumerable<PlaceGridLink> GetCityPlaceNames(SheetsService service, string spreadsheet,
                                                                   string range)
        {
            var response = _retryPolicy.Execute(() => service.Spreadsheets.Values.Get(spreadsheet, range).Execute());

            var values = response.Values;

            var items = new List<PlaceGridLink>();

            if (values == null)
            {
                return items;
            }

            foreach (var row in values)
            {
                items.Add(new PlaceGridLink(row[0].ToString(), row[1].ToString(), Convert.ToInt32(row[2])));
            }

            return items;
        }

        private IEnumerable<ZipGridLink> GetZipCodesInQuadrants(SheetsService service, string spreadsheet,
                                                                       string range)
        {
            var response = _retryPolicy.Execute(() => service.Spreadsheets.Values.Get(spreadsheet, range).Execute());

            var values = response.Values;

            var items = new List<ZipGridLink>();

            if (values == null)
            {
                return items;
            }

            foreach (var row in values)
            {
                items.Add(new ZipGridLink(Convert.ToInt32(row[0]), row[1].ToString(), Convert.ToInt32(row[2])));
            }

            return items;
        }

        private IEnumerable<UspsDeliveryPointLink> GetUspsDeliveryPoints(SheetsService service,
                                                                                string spreadsheet, string range)
        {
            var response = _retryPolicy.Execute(() => service.Spreadsheets.Values.Get(spreadsheet, range).Execute());

            var values = response.Values;

            var items = new List<UspsDeliveryPointLink>();

            if (values == null)
            {
                return items;
            }

            foreach (var row in values)
            {
                items.Add(new UspsDeliveryPointLink(Convert.ToInt32(row[0]), row[1].ToString(), 0, row[2].ToString(),
                                                    Convert.ToDouble(row[6]), Convert.ToDouble(row[7])));
            }

            return items;
        }

        private static Dictionary<string, List<GridLinkable>> BuildGridLinkableLookup(
            IEnumerable<GridLinkable> gridLookup)
        {
            var dictionary = new Dictionary<string, List<GridLinkable>>();

            foreach (var item in gridLookup)
            {
                if (dictionary.ContainsKey(item.Key))
                {
                    dictionary[item.Key].Add(item);
                    continue;
                }

                dictionary.Add(item.Key, new List<GridLinkable> { item });
            }

            return dictionary;
        }

        private IEnumerable<PoBoxAddressCorrection> GetCorrections(SheetsService service, string spreadsheet, string range)
        {
            var exclusions = new Dictionary<string, double[]>
            {
                {"bryce canyon", new[] {397995.510155659, 4170226.5544169028}},
                {"alta", new[] {446372D, 4493558D}},
                {"big water", new[] {440947.64679022622, 4103827.0225703362}},
                {"boulder", new[] {462787.32382167457, 4195803.554941956}}
            };

            var response = _retryPolicy.Execute(() => service.Spreadsheets.Values.Get(spreadsheet, range).Execute());
            var values = response.Values;

            var items = new List<PoBoxAddressCorrection>();

            if (values == null)
            {
                return items;
            }

            foreach (var row in values)
            {
                var city = row[5].ToString().ToLower();

                if (!exclusions.ContainsKey(city))
                {
                    continue;
                }

                var location = exclusions[city];
                var zip5 = Convert.ToInt32(row[7]);
                var zip9 = Convert.ToInt32(row[1]);

                items.Add(new PoBoxAddressCorrection(zip5, zip9, location[0], location[1]));
            }

            return items;
        }
    }
}
