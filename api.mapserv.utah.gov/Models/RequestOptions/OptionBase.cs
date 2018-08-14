using System.ComponentModel;
using api.mapserv.utah.gov.Models.Constants;

namespace api.mapserv.utah.gov.Models.RequestOptions
{
    public class OptionBase
    {
        public OptionBase()
        {
            Format = JsonFormat.None;
        }

        /// <summary>
        ///     The format of the resulting address. esri json will easily parse into an esri.Graphic for display on a map and geojson will easily parse into a feature for use in many open source projects. If this value is omitted, the default json will be returned.
        /// </summary>
        [DefaultValue(JsonFormat.None)]
        public JsonFormat Format { get; set; }
    }
}
