using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Serilog;
using WebAPI.Common.Abstractions;
using WebAPI.Common.Formatters;
using WebAPI.Domain;
using WebAPI.Domain.ArcServerResponse.MapService;

namespace WebAPI.API.Commands.Sgid
{
    public class GetPoBoxLocationsCommand : Command<Dictionary<int, PoBoxAddress>>
    {
        private readonly string _url;

        public GetPoBoxLocationsCommand()
        {
            var gisServer = ConfigurationManager.AppSettings["gis_server_host"];
            _url = $"http://{gisServer}/arcgis/rest/services/WebAPI/PoBoxData/MapServer/";
        }

        public override string ToString()
        {
            return $"GetPoBoxLocationsCommand, {_url}";
        }
        protected override void Execute()
        {
            var poboxes = new Dictionary<int, PoBoxAddress>();

            const int zipCodePoBoxes = 0;
            const int postOffices = 1;

            var codes =  GetZipLocations(zipCodePoBoxes, "ZIP5").Result;
            var moreCodes = GetZipLocations(postOffices, "ZIP").Result;
            foreach (var result in codes)
            {
                var zip = Convert.ToInt32(result.Attributes.Values.First());
                if (poboxes.ContainsKey(zip))
                {
                    continue;
                }
                poboxes.Add(zip, new PoBoxAddress(zip, result.Geometry.X, result.Geometry.Y));
            }

            foreach (var result in moreCodes)
            {
                var zip = Convert.ToInt32(result.Attributes.Values.First());
                if (poboxes.ContainsKey(zip))
                {
                    continue;
                }
                poboxes.Add(zip, new PoBoxAddress(zip, result.Geometry.X, result.Geometry.Y));
            }

            Result = poboxes;
        }

        private async Task<IEnumerable<QueryResponse.QueryResult>> GetZipLocations(int position, string field)
        {
            var queryParams = new[]
            {
                new KeyValuePair<string, string>("where", "1=1"),
                new KeyValuePair<string, string>("f", "json"),
                new KeyValuePair<string, string>("outFields", field),
                new KeyValuePair<string, string>("returnGeometry", "true")
            };

            var formUrl = new FormUrlEncodedContent(queryParams);
            var queryString = await formUrl.ReadAsStringAsync().ConfigureAwait(false);

            var url = $"{_url}{position}/query?{queryString}";
            Log.Verbose("querying {url}", url);

            QueryResponse information = null;
            try
            {
                var response = await App.HttpClient.GetAsync(url).ConfigureAwait(false);
                information = await response.Content
                    .ReadAsAsync<QueryResponse>(new[] {new TextPlainResponseFormatter()}).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "could not query map service for po boxes");
            }

            return information?.Features;
        }
    }
}