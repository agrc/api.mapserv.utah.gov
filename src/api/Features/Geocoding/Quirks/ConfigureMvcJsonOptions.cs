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
    private readonly IOptionsMonitor<JsonOptions> _jsonOptions;
    private readonly ILoggerFactory _log;

    public ConfigureMvcJsonOptions(
        IOptionsMonitor<JsonOptions> jsonOptions,
        ILoggerFactory log) {
        _jsonOptions = jsonOptions;
        _log = log;
    }

    public void Configure(MvcOptions options) {
        var quirkOptions = new JsonSerializerOptions() {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = new AsIsNamingPolicy(),
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
            Converters = {
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase),
                new GeoJsonConverterFactory()
            },
            TypeInfoResolver = new DefaultJsonTypeInfoResolver {
                Modifiers = { new SerializeNullArrayAsEmpty(_log.CreateLogger<SerializeNullArrayAsEmpty>()).ModifyTypeInfo }
            }
        };

        var optionsDefinedInProgram = _jsonOptions.Get(Options.DefaultName);

        options.OutputFormatters.Insert(0, new VersionedJsonOutputFormatter(
            static apiVersion => apiVersion == ApiVersion.Default,
            quirkOptions
        ));

        options.OutputFormatters.Insert(0, new VersionedJsonOutputFormatter(
            static apiVersion => apiVersion > ApiVersion.Default,
            optionsDefinedInProgram.JsonSerializerOptions
        ));
    }
}
