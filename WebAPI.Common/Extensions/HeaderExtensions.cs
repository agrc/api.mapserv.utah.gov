using System;
using System.Net.Http;

namespace WebAPI.Common.Extensions
{
    public static class HeaderExtensions
    {
        /// <summary>
        /// Adds the type header for api request logging.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static HttpResponseMessage AddTypeHeader(this HttpResponseMessage response, Type type)
        {
            response.Content.Headers.Add("X-Type", type.ToString());

            return response;
        } 
    }
}