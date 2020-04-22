using System;
namespace developer.mapserv.utah.gov.Areas.Secure.Formatters
{
    public class ValueFormatter
    {
        public string CommasNoDecimals(int? value, string defaultValue = "0") => value.HasValue ? value.Value.ToString("##,#0") : defaultValue;

        public string DefaultIfNull(int? value, string defaultValue = "0") => value.HasValue ? value.Value.ToString() : defaultValue;
    }
}
