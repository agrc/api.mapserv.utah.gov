using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using AGRC.api.Quirks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetTopologySuite.IO.Converters;

namespace AGRC.api.Geocoding;
public class ConfigureMvcJsonOptions : IConfigureOptions<MvcOptions> {
    private readonly ILoggerFactory _log;

    public ConfigureMvcJsonOptions(IOptionsMonitor<JsonOptions> _, ILoggerFactory log) {
        _log = log;
    }

    public void Configure(MvcOptions options) {
        var defaultOptions = new JsonSerializerOptions() {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
            Converters = {
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase),
                new GeoJsonConverterFactory()
            },
        };

        var quirkOptions = new JsonSerializerOptions() {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = new AsIsNamingPolicy(),
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
            Converters = {
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase),
                new GeoJsonConverterFactory()
            },
            TypeInfoResolver = new DefaultJsonTypeInfoResolver {
                Modifiers = {
                    new SerializableGraphicJsonModifier(_log.CreateLogger<SerializableGraphicJsonModifier>()).ModifyAttributeConverter
                 }
            }
        };

        options.OutputFormatters.Insert(0, new VersionedJsonOutputFormatter(
            static apiVersion => apiVersion == ApiVersion.Default,
            quirkOptions
        ));

        options.OutputFormatters.Insert(0, new VersionedJsonOutputFormatter(
            static apiVersion => apiVersion > ApiVersion.Default,
            defaultOptions
        ));
    }
}
