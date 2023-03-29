using AGRC.api.Features.Converting;
using AGRC.api.Features.Geocoding;
using AGRC.api.Infrastructure;
using AGRC.api.Models.ResponseContracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

#nullable enable
namespace AGRC.api.Filters;
public class JsonpResultFilter : IAsyncResultFilter {
    private readonly IComputeMediator _mediator;
    private readonly ApiVersion _version;

    public JsonpResultFilter(IComputeMediator mediator, IHttpContextAccessor context) {
        _mediator = mediator;
        _version = context.HttpContext!.GetRequestedApiVersion()!;
    }

    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next) {
        if (_version > ApiVersion.Default) {
            await next();

            return;
        }

        if (!context.HttpContext.Request.Query.TryGetValue("callback", out var callback)) {
            await next();

            return;
        }

        if (string.IsNullOrEmpty(callback)) {
            await next();

            return;
        }

        if (context.Result is not ObjectResult response) {
            await next();

            return;
        }

        if (response.Value is ApiResponseContract<SingleGeocodeResponseContract> container) {
            switch (callback.ToString().ToLowerInvariant()) {
                case "geojson": {
                        var command = new GeoJsonFeature.Computation(container, _version);
                        response.Value = await _mediator.Handle(command, default);

                        break;
                    }
                case "esrijson": {
                        var command = new EsriGraphic.Computation(container, _version);
                        response.Value = await _mediator.Handle(command, default);

                        break;
                    }
            }
        }

        await next();
    }
}
