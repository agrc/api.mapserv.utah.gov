using System.IO;
using System.Threading.Tasks;
using api.mapserv.utah.gov.Models.ApiResponses;
using api.mapserv.utah.gov.Models.ResponseObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Distributed;
using ProtoBuf;

namespace api.mapserv.utah.gov.Filters {
    public class CacheResultFilter : IAsyncResultFilter {

        private readonly IDistributedCache _cache;

        public CacheResultFilter(IDistributedCache cache) {
            _cache = cache;
        }

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next) {
            var response = context.Result as ObjectResult;

            var path = context.HttpContext.Request.Path.Value;
            var query = context.HttpContext.Request.Query;
            byte[] proto = null;
            if (response?.Value is ApiResponseContainer<GeocodeAddressApiResponse> geocodeContainer) {
                proto = ToByteArray(geocodeContainer);
            }
            if (response?.Value is ApiResponseContainer container) {
                proto = ToByteArray(container);
            }

            await _cache.SetAsync(path, proto);

            await next();

            return;
        }

        private static byte[] ToByteArray<T>(T obj) {
            byte[] data;
            using (var ms = new MemoryStream()) {
                Serializer.Serialize(ms, obj);
                data = ms.ToArray();
            }

            return data;
        }
    }

}
