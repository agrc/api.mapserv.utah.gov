using System.Text.Json.Serialization;
using AGRC.api.Models;

namespace AGRC.api.Features.Milepost;
public class RouteMilepostResponseContract {
    /// <summary>
    /// The datasource for the result
    /// </summary>
    /// <example>SGID10.TRANSPORTATION.UDOTROUTES_LRS</example>
    public string Source { get; set; }

    /// <summary>
    /// The geographic coordinates along the route for the given milepost measure
    /// </summary>
    public Point Location { get; set; }

    /// <summary>
    /// The route matched
    /// </summary>
    /// <example>Route 0015, Milepost 309.001</example>
    public string MatchRoute { get; set; }

    [JsonIgnore]
    public string InputRouteMilePost { get; set; }
}
