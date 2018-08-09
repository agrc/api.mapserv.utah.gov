using System.Threading.Tasks;
using api.mapserv.utah.gov.Commands.Formatting;
using api.mapserv.utah.gov.Models;
using api.mapserv.utah.gov.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace api.mapserv.utah.gov.Filters
{
    public class JsonOutputFormatResultFilter : IAsyncResultFilter
    {
        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            var format = context.HttpContext.Request.Query["format"];
            if (string.IsNullOrEmpty(format))
            {
                await next().ConfigureAwait(false);

                return;
            }

            var response = context.Result as ObjectResult;

            if (response.Value is ApiResponseContainer<GeocodeAddressApiResponse> container)
            {
                switch (format)
                {
                    case "geojson":
                        {
                            var command = new ConvertToGeoJsonCommand(container);
                            response.Value = CommandExecutor.ExecuteCommand(command);

                            break;
                        }
                    case "esrijson":
                        {
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }

            await next().ConfigureAwait(false);
        }
    }
}
