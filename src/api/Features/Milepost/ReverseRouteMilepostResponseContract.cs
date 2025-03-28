namespace ugrc.api.Features.Milepost;
public class ReverseRouteMilepostResponseContract {
    private string _routeName = string.Empty;

    /// <summary>
    /// Gets or sets the name of the route.
    /// </summary>
    /// <value>
    /// The name of the route with the leading zeros removed.
    /// </value>
    public string Route {
        get {
            if (string.IsNullOrEmpty(_routeName?.Trim())) {
                return string.Empty;
            }

            return _routeName.TrimStart('0').TrimEnd('M');
        }
        set => _routeName = value;
    }

    /// <summary>
    ///     Gets or sets the distance away from the input point.
    /// </summary>
    /// <value>
    ///     The distance away from the input point in meters. -1 for not found. Rounded to two decimal places
    /// </value>
    public double OffsetMeters {
        get => Math.Round(field, 2);
        set;
    }

    /// <summary>
    ///     Gets or sets the milepost.
    /// </summary>
    /// <value>
    ///     The closest milepost value rounded to three decimal places.
    /// </value>
    public double Milepost {
        get => Math.Round(field, 3);
        set;
    }

    /// <summary>
    /// Gets or sets the side.
    /// </summary>
    /// <value>
    /// The side of the road that the point was on.
    /// </value>
    public string Side => _routeName.Contains('P', StringComparison.InvariantCultureIgnoreCase)
        ? "increasing" : "decreasing";

    public bool? Dominant { get; set; }

    public IEnumerable<ReverseRouteMilepostResponseContract>? Candidates { get; set; }

    public bool ShouldSerializeCandidates() {
        if (Candidates == null)
            return false;

        return Candidates.Any();
    }
}
