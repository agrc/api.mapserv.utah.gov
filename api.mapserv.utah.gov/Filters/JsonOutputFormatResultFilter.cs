using System.Threading.Tasks;
using api.mapserv.utah.gov.Features.Converting;
using api.mapserv.utah.gov.Models;
using api.mapserv.utah.gov.Models.ResponseObjects;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace api.mapserv.utah.gov.Filters
{
    public class JsonOutputFormatResultFilter : IAsyncResultFilter
    {
        private readonly IMediator _mediator;

        public JsonOutputFormatResultFilter(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            var format = context.HttpContext.Request.Query["format"];
            if (string.IsNullOrEmpty(format))
            {
                await next();

                return;
            }

            var response = context.Result as ObjectResult;

            if (response.Value is ApiResponseContainer<GeocodeAddressApiResponse> container)
            {
                switch (format.ToString().ToLowerInvariant())
                {
                    case "geojson":
                        {
                            var command = new GeoJsonFeature.Command(container);
                            response.Value = await _mediator.Send(command);

                            break;
                        }
                    case "esrijson":
                        {
                            var command = new EsriGraphic.Command(container);
                            response.Value = await _mediator.Send(command);

                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }

            await next();
        }
    }
}
