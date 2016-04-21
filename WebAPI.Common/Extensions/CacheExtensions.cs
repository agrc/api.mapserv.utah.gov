using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace WebAPI.Common.Extensions
{
    public static class CacheExtensions
    {
        public static HttpResponseMessage AddCache(this HttpResponseMessage response)
        {
            var cacheTime = DateTime.Now;
            var cacheSeconds = 0;

#if !DEBUG
            cacheSeconds = 30;
            cacheTime = cacheTime.AddSeconds(cacheSeconds);
#endif

            response.Content.Headers.Expires = new DateTimeOffset(cacheTime);
            response.Headers.CacheControl = new CacheControlHeaderValue
            {
                Public = true,
                MaxAge = TimeSpan.FromSeconds(cacheSeconds)
            };

            return response;
        }
    }
}