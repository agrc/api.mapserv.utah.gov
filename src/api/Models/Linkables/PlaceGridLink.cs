namespace AGRC.api.Models.Linkables;
public class PlaceGridLink(string place, string grid, int weight) : GridLinkable(grid, weight) {
    public string City { get; set; } = place;

    public override string Key => City.ToLowerInvariant();
}
