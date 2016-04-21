using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;

namespace WebAPI.API.Negotiators
{
    public class JsonContentNegotiator : IContentNegotiator
    {
        public ContentNegotiationResult Negotiate(Type type, HttpRequestMessage request,
                                                  IEnumerable<MediaTypeFormatter> formatters)
        {
            var result = new ContentNegotiationResult(new JsonMediaTypeFormatter(),
                                                      new MediaTypeHeaderValue("application/json"));
            return result;
        }
    }
}