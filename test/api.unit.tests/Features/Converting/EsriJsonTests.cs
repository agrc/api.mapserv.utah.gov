using AGRC.api.Features.Converting;
using AGRC.api.Features.Geocoding;
using AGRC.api.Infrastructure;
using AGRC.api.Models;
using AGRC.api.Models.ResponseContracts;

namespace api.tests.Features.Converting;
public class EsriJsonTests {
    private readonly IComputationHandler<EsriGraphic.Computation, ApiResponseContract<EsriGraphic.SerializableGraphic>> _handler =
        new EsriGraphic.Handler(new Mock<ILogger> { DefaultValue = DefaultValue.Mock }.Object);

    [Fact]
    public async Task Should_convert_to_esri_graphic() {
        var responseContainer = new ApiResponseContract<SingleGeocodeResponseContract> {
            Result = new SingleGeocodeResponseContract {
                Candidates = null,
                InputAddress = "Input Address",
                Location = new Point(1, 1),
                Locator = "Centerlines",
                MatchAddress = "Matched Address",
                Score = 100,
                Wkid = 26912,
                ScoreDifference = null
            },
            Status = 200
        };

        var request = new EsriGraphic.Computation(responseContainer, new ApiVersion(2, 0));
        var result = await _handler.Handle(request, new CancellationToken());

        var options = new JsonSerializerOptions {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
        };

        const string graphic = "{\"attributes\":{\"score\":100,\"locator\":\"Centerlines\",\"matchAddress\":\"Matched Address\",\"inputAddress\":\"Input Address\"},\"geometry\":{\"x\":1,\"y\":1,\"type\":\"point\",\"spatialReference\":{\"wkid\":26912}}}";
        var resultJson = JsonSerializer.Serialize(result.Result, options);

        resultJson.ShouldBe(graphic);
    }

    [Fact]
    public async Task Should_handle_address_not_found() {
        var responseContainer = new ApiResponseContract<SingleGeocodeResponseContract> {
            Result = null,
            Message = "No address candidates found with a score of 70 or better.",
            Status = 404
        };

        var request = new EsriGraphic.Computation(responseContainer, new ApiVersion(2, 0));
        var result = await _handler.Handle(request, new CancellationToken());

        result.Result.ShouldBeNull();
        result.Message.ShouldBe(responseContainer.Message);
        result.Status.ShouldBe(responseContainer.Status);
    }
}
