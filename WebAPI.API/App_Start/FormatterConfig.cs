using System.Net.Http.Formatting;
using Newtonsoft.Json;

namespace WebAPI.API
{
    public static class FormatterConfig
    {
        public static void RegisterFormatters(MediaTypeFormatterCollection formatters)
        {
            formatters.Remove(formatters.XmlFormatter);
            
            var json = formatters.JsonFormatter;
            json.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
        }
    }
}