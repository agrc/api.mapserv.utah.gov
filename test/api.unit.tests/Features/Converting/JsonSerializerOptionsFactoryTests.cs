using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using AGRC.api.Features.Converting;
using AGRC.api.Geocoding;
using NetTopologySuite.IO.Converters;

namespace api.tests.Converting;

public class JsonSerializerOptionsFactoryTests {
    public JsonSerializerOptionsFactory Factory { get; }
    public JsonSerializerOptionsFactoryTests() {
        Factory = new JsonSerializerOptionsFactory();
    }

    [Fact]
    public void Should_provide_quirky_options_for_version_1() {
        var options = Factory.GetSerializerOptionsFor(new ApiVersion(1, 0));

        options.PropertyNamingPolicy.ShouldBe(JsonNamingPolicy.CamelCase);
        options.DictionaryKeyPolicy.ShouldBeOfType<AsIsNamingPolicy>();
        options.DefaultIgnoreCondition.ShouldBe(JsonIgnoreCondition.WhenWritingDefault);
        options.Converters.ShouldContain(c => c.GetType() == typeof(JsonStringEnumConverter));
        options.Converters.ShouldContain(c => c.GetType() == typeof(GeoJsonConverterFactory));
        options.TypeInfoResolver.ShouldBeOfType<DefaultJsonTypeInfoResolver>();
    }
    [Fact]
    public void Should_provide_default_options_for_version_2() {
        var options = Factory.GetSerializerOptionsFor(new ApiVersion(2, 0));

        options.PropertyNamingPolicy.ShouldBe(JsonNamingPolicy.CamelCase);
        options.DictionaryKeyPolicy.ShouldBe(JsonNamingPolicy.CamelCase);
        options.DefaultIgnoreCondition.ShouldBe(JsonIgnoreCondition.WhenWritingDefault);
        options.Converters.ShouldContain(c => c.GetType() == typeof(JsonStringEnumConverter));
        options.Converters.ShouldContain(c => c.GetType() == typeof(GeoJsonConverterFactory));
        options.TypeInfoResolver.ShouldBeNull();
    }
    [Fact]
    public void Should_provide_default_options_for_versions_above_1() {
        var options = Factory.GetSerializerOptionsFor(new ApiVersion(3, 0));

        options.PropertyNamingPolicy.ShouldBe(JsonNamingPolicy.CamelCase);
        options.DictionaryKeyPolicy.ShouldBe(JsonNamingPolicy.CamelCase);
        options.DefaultIgnoreCondition.ShouldBe(JsonIgnoreCondition.WhenWritingDefault);
        options.Converters.ShouldContain(c => c.GetType() == typeof(JsonStringEnumConverter));
        options.Converters.ShouldContain(c => c.GetType() == typeof(GeoJsonConverterFactory));
        options.TypeInfoResolver.ShouldBeNull();
    }
}
