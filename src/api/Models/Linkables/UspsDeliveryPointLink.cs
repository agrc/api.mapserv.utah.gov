namespace AGRC.api.Models.Linkables;
public class UspsDeliveryPointLink(int zip, string grid, string place, double x, double y) : ZipGridLink(zip, grid, 0) {
    public string MatchAddress { get; set; } = place;
    public double X { get; set; } = x;
    public double Y { get; set; } = y;
}
