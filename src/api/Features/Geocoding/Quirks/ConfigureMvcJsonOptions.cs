using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetTopologySuite.IO.Converters;

namespace AGRC.api.Geocoding;

public class ConfigureMvcJsonOptions : IConfigureOptions<MvcOptions> {
    private readonly IOptionsMonitor<JsonOptions> _jsonOptions;
    private readonly ILoggerFactory _loggerFactory;

    public ConfigureMvcJsonOptions(
        IOptionsMonitor<JsonOptions> jsonOptions,
        ILoggerFactory loggerFactory) {
        _jsonOptions = jsonOptions;
        _loggerFactory = loggerFactory;
    }

    public void Configure(MvcOptions options) {
        var quirkOptions = new JsonSerializerOptions() {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = new AsIsNamingPolicy(),
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
        };

        quirkOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        quirkOptions.Converters.Add(new GeoJsonConverterFactory());

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
