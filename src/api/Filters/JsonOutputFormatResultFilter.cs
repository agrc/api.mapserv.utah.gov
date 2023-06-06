using AGRC.api.Features.Converting;
using AGRC.api.Features.Geocoding;
using AGRC.api.Infrastructure;
using AGRC.api.Models.ResponseContracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AGRC.api.Filters;
public class JsonOutputFormatResultFilter : IAsyncResultFilter {
    private readonly IComputeMediator _mediator;


    public JsonOutputFormatResultFilter(IComputeMediator mediator) {
        _mediator = mediator;
    }

    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next) {
        if (!context.HttpContext.Request.Query.TryGetValue("format", out var format)) {
            await next();

            return;
        }

        if (string.IsNullOrEmpty(format)) {
            await next();

            return;
        }

        if (context.Result is not ObjectResult response) {
            await next();

            return;
        }

        var version = context.HttpContext!.GetRequestedApiVersion();
        if (response.Value is ApiResponseContract<SingleGeocodeResponseContract> container) {
            switch (format.ToString().ToLowerInvariant()) {
                case "geojson": {
                        var command = new GeoJsonFeature.Computation(container, version);
                        response.Value = await _mediator.Handle(command, default);

                        break;
                    }
                case "esrijson": {
                        var command = new EsriGraphic.Computation(container, version);
                        response.Value = await _mediator.Handle(command, default);

                        break;
                    }
            }
        }

        await next();
    }
}
