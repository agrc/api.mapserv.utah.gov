using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Logging;

namespace AGRC.api.Geocoding;
public class VersionedJsonInputFormatter : SystemTextJsonInputFormatter {
    private readonly Func<ApiVersion, bool> _isSatisfiedBy;

    public VersionedJsonInputFormatter(Func<ApiVersion, bool> isSatisfiedBy, JsonOptions options, ILogger<VersionedJsonInputFormatter> logger)
        : base(options, logger) {
        _isSatisfiedBy = isSatisfiedBy;
    }

    public override bool CanRead(InputFormatterContext context) =>
        base.CanRead(context) &&
        context.HttpContext.GetRequestedApiVersion() is ApiVersion apiVersion &&
        _isSatisfiedBy(apiVersion);
}

public class VersionedJsonOutputFormatter : SystemTextJsonOutputFormatter {
    private readonly Func<ApiVersion, bool> _isSatisfiedBy;

    public VersionedJsonOutputFormatter(Func<ApiVersion, bool> isSatisfiedBy, JsonSerializerOptions jsonSerializerOptions)
        : base(jsonSerializerOptions) {
        _isSatisfiedBy = isSatisfiedBy;
    }

    public override bool CanWriteResult(OutputFormatterCanWriteContext context) =>
        base.CanWriteResult(context) &&
        context.HttpContext.GetRequestedApiVersion() is ApiVersion apiVersion && _isSatisfiedBy(apiVersion);
}
