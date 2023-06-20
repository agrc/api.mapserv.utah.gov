namespace AGRC.api.Models.Linkables;
public class ZipGridLink(int zip, string grid, int weight) : GridLinkable(grid, weight) {
    public int ZipCode { get; set; } = zip;

    public override string Key => ZipCode.ToString(CultureInfo.InvariantCulture);
}
