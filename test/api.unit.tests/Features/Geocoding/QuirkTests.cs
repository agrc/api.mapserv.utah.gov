using System.IO;
using System.Text;
using System.Text.Json.Serialization;
using AGRC.api.Geocoding;
using AGRC.api.Quirks;
using NetTopologySuite.IO.Converters;

namespace api.tests.Features.Geocoding;

public class QuirkTests {
    [Theory]
    [InlineData("Same", "Same")]
    [InlineData("same", "same")]
    [InlineData("SAME", "SAME")]
    public void As_is_naming_should_pass_through(string input, string result) {
        var policy = new AsIsNamingPolicy();
        policy.ConvertName(input).ShouldBe(result);
    }
    [Fact]
    public void Esri_json_converter_should_skip_nulls() {
        var converter = new EsriJsonAttributesConverter();
        using var stream = new MemoryStream();
        using var writer = new Utf8JsonWriter(stream);
        var dictionary = new Dictionary<string, object> {
            {"standardizedAddress", null},
            {"scoreDifference", null},
            {"candidates", null}
        };
        converter.Write(writer, dictionary, new JsonSerializerOptions() {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = new AsIsNamingPolicy(),
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
            Converters = {
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase),
                new GeoJsonConverterFactory()
            }
        });
        writer.Flush();

        Encoding.UTF8.GetString(stream.ToArray()).ShouldBe("""{"scoreDifference":-1,"candidates":[]}""");
    }
    [Fact]
    public void Esri_json_converter_should_not_reset_existing_values() {
        var converter = new EsriJsonAttributesConverter();
        using var stream = new MemoryStream();
        using var writer = new Utf8JsonWriter(stream);
        var dictionary = new Dictionary<string, object> {
            {"standardizedAddress", "not null"},
            {"scoreDifference", 10},
            {"candidates", new[]{1,2,3}}
        };
        converter.Write(writer, dictionary, new JsonSerializerOptions() {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = new AsIsNamingPolicy(),
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
            Converters = {
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase),
                new GeoJsonConverterFactory()
            }
        });
        writer.Flush();

        Encoding.UTF8.GetString(stream.ToArray()).ShouldBe("""{"standardizedAddress":"not null","scoreDifference":10,"candidates":[1,2,3]}""");
    }
    [Fact]
    public void Esri_json_converter_is_for_writing_only() {
        var converter = new EsriJsonAttributesConverter();
        var reader = new Utf8JsonReader();

        try {
            converter.Read(ref reader, typeof(Dictionary<string, object>), new JsonSerializerOptions());
        } catch (Exception ex) {
            // this is expected
            ex.ShouldBeOfType<NotImplementedException>();
        }
    }
}
