using ugrc.api.Models.ArcGis;

namespace api.tests.Models.ArcGis;
public class RestErrorableTests {
    [Fact]
    public void Should_serialize_error_on_failure() {
        const string geometryServiceError = """{"error":{"code": 500,"message": "Error processing request","details": [ ]}}""";

        var obj = JsonSerializer.Deserialize<ServiceInformation>(geometryServiceError, new JsonSerializerOptions {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        obj.IsSuccessful.ShouldBe(false);
        obj.Error.Code.ShouldBe(500);
        obj.Error.Message.ShouldBe("Error processing request");
    }
}
