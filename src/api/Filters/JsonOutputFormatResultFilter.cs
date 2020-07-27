using System.Threading.Tasks;
using AGRC.api.Features.Converting;
using AGRC.api.Features.Geocoding;
using AGRC.api.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using AGRC.api.Models.ResponseContracts;

namespace AGRC.api.Filters {
    public class JsonOutputFormatResultFilter : IAsyncResultFilter {
        private readonly IComputeMediator _mediator;

        public JsonOutputFormatResultFilter(IComputeMediator mediator) {
            _mediator = mediator;
        }

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next) {
            var format = context.HttpContext.Request.Query["format"];
            if (string.IsNullOrEmpty(format)) {
                await next();

                return;
            }

            var response = context.Result as ObjectResult;

            if (response.Value is ApiResponseContract<SingleGeocodeResponseContract> container) {
                switch (format.ToString().ToLowerInvariant()) {
                    case "geojson": {
                        var command = new GeoJsonFeature.Computation(container);
                        response.Value = await _mediator.Handle(command, default);

                        break;
                    }
                    case "esrijson": {
                        var command = new EsriGraphic.Computation(container);
                        response.Value = await _mediator.Handle(command, default);

                        break;
                    }
                    default: {
                        break;
                    }
                }
            }

            await next();
        }
    }
}
