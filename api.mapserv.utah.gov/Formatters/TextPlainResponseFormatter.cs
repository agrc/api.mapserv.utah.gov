using System;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;

namespace api.mapserv.utah.gov.Formatters
{
    public class TextPlainResponseFormatter : JsonMediaTypeFormatter
    {
        public TextPlainResponseFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/plain"));
        }
    }
}
