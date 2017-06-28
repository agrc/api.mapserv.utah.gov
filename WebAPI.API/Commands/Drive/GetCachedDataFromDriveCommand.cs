using System;
using System.Collections.Generic;
using System.IO;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using WebAPI.Common.Abstractions;
using WebAPI.Domain;
using WebAPI.Domain.Linkers;

namespace WebAPI.API.Commands.Drive
{
    public class GetCachedDataFromDriveCommand : Command<DriveDataModel>
    {
        private const string ApplicationName = "Geocoding Cache";
        private static readonly string[] Scopes = {SheetsService.Scope.Spreadsheets};

        public override string ToString()
        {
            return "CreateCsvFromDriveCommand";
        }

        protected override void Execute()
        {
            GoogleCredential credential;

            var file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "client_secret.json");
            using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream).CreateScoped(Scopes);
            }

            using (var service = new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
                GZipEnabled = true
            }))
            {

                // Define request parameters.
                var places = GetCityPlaceNames(service, "1Dc77HLVn9YXgY1JkcqqaMh1Tm0cemwXxFfyHb7dP0Cs", "A2:C");
                var zips = GetZipCodesInQuadrants(service, "1HX47abPQ24LqB8iUHu6HTSmTnbU1YN9WvxO6BDsHCLg", "A2:C");
                var deliveryPoints = GetUspsDeliveryPoints(service, "1rgqNweBxiqWVwxTlMWgX-sN8IJguvlUcY0yOvmI8JuQ",
                    "A2:H");
                var corrections = GetCorrections(service, "1ZqXyQflKQ8q-sBHCHLaYaQXY90aOFU0JPc4-M_41GM0", "A2:H");

                Result = new DriveDataModel
                {
                    PlaceGrids = BuildGridLinkableLookup(places),
                    ZipCodesGrids = BuildGridLinkableLookup(zips),
                    UspsDeliveryPoints = BuildGridLinkableLookup(deliveryPoints),
                    PoBoxExclusions = corrections
                };
            }
        }

        private static IEnumerable<PlaceGridLink> GetCityPlaceNames(SheetsService service, string spreadsheet,
            string range)
        {
            var request = service.Spreadsheets.Values.Get(spreadsheet, range);

            var response = request.Execute();
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

        private static IEnumerable<ZipGridLink> GetZipCodesInQuadrants(SheetsService service, string spreadsheet,
            string range)
        {
            var request = service.Spreadsheets.Values.Get(spreadsheet, range);

            var response = request.Execute();
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

        private static IEnumerable<UspsDeliveryPointLink> GetUspsDeliveryPoints(SheetsService service,
            string spreadsheet, string range)
        {
            var request = service.Spreadsheets.Values.Get(spreadsheet, range);

            var response = request.Execute();
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

                dictionary.Add(item.Key, new List<GridLinkable> {item});
            }

            return dictionary;
        }

        private static IEnumerable<PoBoxAddressCorrection> GetCorrections(SheetsService service, string spreadsheet, string range)
        {
            var exclusions = new Dictionary<string, double[]>
            {
                {"bryce canyon", new[] {397995.510155659, 4170226.5544169028}},
                {"alta", new[] {446372D, 4493558D}},
                {"big water", new[] {440947.64679022622, 4103827.0225703362}},
                {"boulder", new[] {462787.32382167457, 4195803.554941956}}
            };

            var request = service.Spreadsheets.Values.Get(spreadsheet, range);

            var response = request.Execute();
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

    public class DriveDataModel
    {
        public Dictionary<string, List<GridLinkable>> PlaceGrids { get; set; }

        public Dictionary<string, List<GridLinkable>> ZipCodesGrids { get; set; }

        public Dictionary<string, List<GridLinkable>> UspsDeliveryPoints { get; set; }

        public IEnumerable<PoBoxAddressCorrection> PoBoxExclusions { get; set; }
    }
}