using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using NetTopologySuite.IO.Converters;
using ugrc.api.Geocoding;
using ugrc.api.Quirks;

namespace ugrc.api.Features.Converting;

public interface IJsonSerializerOptionsFactory {
    JsonSerializerOptions GetSerializerOptionsFor(ApiVersion version);
}

public class JsonSerializerOptionsFactory : IJsonSerializerOptionsFactory {
    private readonly JsonSerializerOptions _defaultOptions;
    private readonly JsonSerializerOptions _quirkOptions;

    public JsonSerializerOptionsFactory() {
        _defaultOptions = new JsonSerializerOptions() {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
            Converters = {
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase),
                new GeoJsonConverterFactory()
            },
        };

        _quirkOptions = new JsonSerializerOptions() {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = new AsIsNamingPolicy(),
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
            Converters = {
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase),
                new GeoJsonConverterFactory()
            },
            TypeInfoResolver = new DefaultJsonTypeInfoResolver {
                Modifiers = {
                    new SerializableGraphicJsonModifier().ModifyAttributeConverter
                }
            }
        };
    }

    public JsonSerializerOptions GetSerializerOptionsFor(ApiVersion version) => version.MajorVersion switch {
        1 => _quirkOptions,
        _ => _defaultOptions
    };
}
