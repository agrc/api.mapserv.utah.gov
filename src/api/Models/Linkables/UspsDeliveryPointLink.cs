#nullable enable
namespace AGRC.api.Models.Linkables;
public class UspsDeliveryPointLink : ZipGridLink {
    public string MatchAddress { get; set; }
    public double X { get; set; }
    public double Y { get; set; }

    public UspsDeliveryPointLink(int zip, string grid, string place, double x, double y) :
        base(zip, grid, 0) {
        MatchAddress = place;
        X = x;
        Y = y;
    }
}
