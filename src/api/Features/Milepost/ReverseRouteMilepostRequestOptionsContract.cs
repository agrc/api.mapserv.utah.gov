using System.ComponentModel;
using AGRC.api.Models.RequestOptionContracts;

namespace AGRC.api.Features.Milepost;
public class ReverseRouteMilepostRequestOptionsContract : ProjectableOptions {
    /// <summary>
    /// The radius around the input location in meters.
    /// </summary>
    /// <example>
    /// 100
    /// </example>
    [DefaultValue(100)]
    public double Buffer { get; set; } = 100;

    /// <summary>
    /// The spatial reference well known id.
    /// </summary>
    /// <example>
    /// 4326
    /// </example>
    [DefaultValue(26912)]
    public int WkId { get; set; }

    /// <summary>
    /// Include ramps in the results
    /// </summary>
    /// <example>
    /// true
    /// </example>
    [DefaultValue(false)]
    public bool IncludeRampSystem { get; set; }

    /// <summary>
    /// The number of candidates to return beside the highest match candidate.
    /// </summary>
    /// <example>
    /// 2
    /// </example>
    [DefaultValue(0)]
    public int Suggest { get; set; }
}
