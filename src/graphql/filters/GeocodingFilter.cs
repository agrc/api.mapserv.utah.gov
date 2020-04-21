using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using api.mapserv.utah.gov.Models.ApiResponses;
using api.mapserv.utah.gov.Models.ResponseObjects;
using NetTopologySuite.Geometries;

namespace graphql.filters {
    public class GeocodingFilter {
        public int? Zip { get; set; }
        public string Place { get; set; }
        public string Street { get; set; }

        public bool HasFilter() {
            if (string.IsNullOrEmpty(Street)) {
                return false;
            }

            return !string.IsNullOrEmpty(Place) || (Zip.HasValue && Zip.Value > 8000);
        }

        public async Task<Point> GeocodeAsync(HttpClient httpClient) {
            var response = await httpClient.GetAsync($"http://api.mapserv.utah.gov/api/v1/geocode/{Street}/{(string.IsNullOrEmpty(Place) ? Zip.ToString() : Place)}?spatialReference=26912&apiKey=AGRC-Dev",
            HttpCompletionOption.ResponseHeadersRead);

            if (!response.IsSuccessStatusCode) {
                return null;
            }

            using var responseStream = await response.Content.ReadAsStreamAsync();

            var result = await JsonSerializer.DeserializeAsync<ApiResponseContainer<GeocodeAddressApiResponse>>(responseStream, new JsonSerializerOptions {
                PropertyNameCaseInsensitive = true,
            });

            if (result.Status != 200) {
                return null;
            }

            var coordinate = new Coordinate(result.Result.Location.X, result.Result.Location.Y);
            var point = new Point(coordinate) {
                SRID = 26912
            };

            return point;
        }
    }
}
