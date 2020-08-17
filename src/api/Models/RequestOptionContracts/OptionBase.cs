using System.ComponentModel;
using AGRC.api.Models.Constants;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AGRC.api.Models.RequestOptionContracts {
    public class OptionBase {
        /// <summary>
        /// There are three output formats for the resulting street and zone geocoding. The **default** being empty.
        /// `esrijson` will parse into an `esri.Graphic` for mapping purposes and `geojson` will format as a
        /// [feature](https://tools.ietf.org/html/rfc7946#section-3.2). If this value is omitted, the default json will
        ///  be returned.
        /// </summary>
        [DefaultValue(JsonFormat.None)]
        [JsonConverter(typeof(StringEnumConverter))]
        public JsonFormat Format { get; set; } = JsonFormat.None;
    }
}
