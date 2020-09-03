using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using AGRC.api.Features.Converting;
using AGRC.api.Features.Geocoding;
using AGRC.api.Infrastructure;
using AGRC.api.Models.ResponseContracts;
using GeoJSON.Net.Feature;
using Shouldly;
using Xunit;

namespace api.tests.Features.Converting {
    public class GeoJsonTests {
        private readonly IComputationHandler<GeoJsonFeature.Computation, ApiResponseContract<Feature>> _handler =
            new GeoJsonFeature.Handler();

        [Fact]
        public async Task Should_convert_to_geojson_feature() {
            var responseContainer = new ApiResponseContract<SingleGeocodeResponseContract> {
                Result = new SingleGeocodeResponseContract {
                    Candidates = null,
                    InputAddress = "Input Address",
                    Location = new AGRC.api.Models.Point {
                        X = 1,
                        Y = 1
                    },
                    Locator = "Centerlines",
                    MatchAddress = "Matched Address",
                    Score = 100,
                    Wkid = 26912,
                    ScoreDifference = null
                },
                Status = 200
            };

            var request = new GeoJsonFeature.Computation(responseContainer);
            var result = await _handler.Handle(request, new CancellationToken());

            var options = new JsonSerializerOptions {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
            };

            options.Converters.Add(new JsonStringEnumConverter());

            var feature = "{\"type\":\"Feature\",\"geometry\":{\"coordinates\":[1,1],\"type\":\"Point\"},\"properties\":{\"location\":{\"x\":1,\"y\":1},\"score\":100,\"locator\":\"Centerlines\",\"matchAddress\":\"Matched Address\",\"inputAddress\":\"Input Address\"}}";
            var resultJson = JsonSerializer.Serialize(result.Result, options);

            resultJson.ShouldBe(feature);
        }

        [Fact]
        public async Task Should_handle_address_not_found() {
            var responseContainer = new ApiResponseContract<SingleGeocodeResponseContract> {
                Result = null,
                Message = "No address candidates found with a score of 70 or better.",
                Status = 404
            };

            var request = new GeoJsonFeature.Computation(responseContainer);
            var result = await _handler.Handle(request, new CancellationToken());

            result.Result.ShouldBeNull();
            result.Message.ShouldBe(responseContainer.Message);
            result.Status.ShouldBe(responseContainer.Status);
        }
    }
}
