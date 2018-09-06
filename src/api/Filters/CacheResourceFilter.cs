using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using api.mapserv.utah.gov.Models.ApiResponses;
using api.mapserv.utah.gov.Models.ResponseObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Distributed;
using ProtoBuf;

namespace api.mapserv.utah.gov.Filters {
    public class CacheResourceFilter : IAsyncResourceFilter {
        private readonly IDistributedCache _cache;

        public CacheResourceFilter(IDistributedCache cache) {
            _cache = cache;
        }
        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next) {
            var path = context.HttpContext.Request.Path.Value;
            var query = context.HttpContext.Request.Query;

            // TODO: sort path/query string to normalize cache keys

            var value = await _cache.GetAsync(path);

            // if empty it's a new request. CacheResultFilter will cache it for next time
            if ((value?.Length ?? 0) < 1) {
                await next();

                return;
            }

            // return cache object
            using (var stream = new MemoryStream(value)) {
                context.Result = new ObjectResult(Serializer.Deserialize<ApiResponseContainer<GeocodeAddressApiResponse>>(stream));

                return;
            }

            // await next();
        }
    }
}
