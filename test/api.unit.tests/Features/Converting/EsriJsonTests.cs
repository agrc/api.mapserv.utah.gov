using AGRC.api.Features.Converting;
using AGRC.api.Features.Geocoding;
using AGRC.api.Infrastructure;
using AGRC.api.Models;
using EsriJson.Net;
using static AGRC.api.Features.Converting.EsriGraphic;

namespace api.tests.Features.Converting;
public class EsriJsonTests {
    private readonly IComputationHandler<EsriGraphic.Computation, EsriGraphic.SerializableGraphic> _handler =
        new EsriGraphic.Handler(new Mock<ILogger> { DefaultValue = DefaultValue.Mock }.Object);

    [Fact]
    public async Task Should_convert_to_esri_graphic() {
        var responseContainer = new SingleGeocodeResponseContract {
            Candidates = null,
            InputAddress = "Input Address",
            Location = new Point(1, 1),
            Locator = "Centerlines",
            MatchAddress = "Matched Address",
            Score = 100,
            Wkid = 26912,
            ScoreDifference = null
        };

        var request = new EsriGraphic.Computation(responseContainer, new ApiVersion(2, 0));
        var result = await _handler.Handle(request, new CancellationToken());

        var options = new JsonSerializerOptions {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
        };

        const string graphic = """{"attributes":{"score":100,"locator":"Centerlines","matchAddress":"Matched Address","inputAddress":"Input Address"},"geometry":{"x":1,"y":1,"type":"point","spatialReference":{"wkid":26912}}}""";
        var resultJson = JsonSerializer.Serialize(result, options);

        resultJson.ShouldBe(graphic);
    }

    [Fact]
    public async Task Should_handle_address_not_found() {
        var responseContainer = new SingleGeocodeResponseContract();

        var request = new EsriGraphic.Computation(responseContainer, new ApiVersion(2, 0));
        var result = await _handler.Handle(request, new CancellationToken());

        result.ShouldBeEquivalentTo(new SerializableGraphic(new Graphic(null, null)));
    }
}
