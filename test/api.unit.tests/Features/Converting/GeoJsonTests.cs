
using AGRC.api.Features.Converting;
using AGRC.api.Features.Geocoding;
using AGRC.api.Infrastructure;
using AGRC.api.Models.ResponseContracts;
using NetTopologySuite.Features;
using NetTopologySuite.IO.Converters;

namespace api.tests.Features.Converting;
public class GeoJsonTests {
    private readonly IComputationHandler<GeoJsonFeature.Computation, ApiResponseContract<Feature>> _handler =
        new GeoJsonFeature.Handler(new Mock<ILogger>() { DefaultValue = DefaultValue.Mock }.Object);

    [Fact]
    public async Task Should_convert_to_geojson_feature_v2() {
        var responseContainer = new ApiResponseContract<SingleGeocodeResponseContract> {
            Result = new SingleGeocodeResponseContract {
                Candidates = null,
                InputAddress = "Input Address",
                Location = new AGRC.api.Models.Point(1, 1),
                Locator = "Centerlines",
                MatchAddress = "Matched Address",
                Score = 100,
                Wkid = 26912,
                ScoreDifference = null
            },
            Status = 200
        };

        var request = new GeoJsonFeature.Computation(responseContainer, new ApiVersion(2, 0));
        var result = await _handler.Handle(request, new CancellationToken());

        var options = new JsonSerializerOptions {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
        };

        options.Converters.Add(new JsonStringEnumConverter());
        options.Converters.Add(new GeoJsonConverterFactory());

        const string feature = "{\"type\":\"Feature\",\"geometry\":{\"type\":\"Point\",\"coordinates\":[1,1]},\"properties\":{\"srid\":26912,\"score\":100,\"locator\":\"Centerlines\",\"matchAddress\":\"Matched Address\",\"inputAddress\":\"Input Address\"}}";
        var resultJson = JsonSerializer.Serialize(result.Result, options);

        resultJson.ShouldBe(feature);
    }

    [Fact]
    public async Task Should_convert_to_geojson_feature_v1() {
        var responseContainer = new ApiResponseContract<SingleGeocodeResponseContract> {
            Result = new SingleGeocodeResponseContract {
                Candidates = null,
                InputAddress = "Input Address",
                Location = new AGRC.api.Models.Point(1, 1),
                Locator = "Centerlines",
                MatchAddress = "Matched Address",
                Score = 100,
                Wkid = 26912,
                ScoreDifference = null
            },
            Status = 200
        };

        var request = new GeoJsonFeature.Computation(responseContainer, new ApiVersion(1, 0));
        var result = await _handler.Handle(request, new CancellationToken());

        var options = new JsonSerializerOptions {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
        };

        options.Converters.Add(new JsonStringEnumConverter());
        options.Converters.Add(new GeoJsonConverterFactory());

        const string feature = "{\"type\":\"Feature\",\"geometry\":{\"type\":\"Point\",\"coordinates\":[1,1]},\"properties\":{\"Location\":{\"x\":1,\"y\":1},\"Score\":100,\"Locator\":\"Centerlines\",\"MatchAddress\":\"Matched Address\",\"InputAddress\":\"Input Address\",\"AddressGrid\":null,\"ScoreDifference\":-1,\"Wkid\":26912,\"Candidates\":[]}}";
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

        var request = new GeoJsonFeature.Computation(responseContainer, new ApiVersion(1, 0));
        var result = await _handler.Handle(request, new CancellationToken());

        result.Result.ShouldBeNull();
        result.Message.ShouldBe(responseContainer.Message);
        result.Status.ShouldBe(responseContainer.Status);
    }
}
