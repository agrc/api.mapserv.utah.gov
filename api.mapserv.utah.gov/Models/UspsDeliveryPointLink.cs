namespace api.mapserv.utah.gov.Models
{
    public class UspsDeliveryPointLink : ZipGridLink
    {
        public string MatchAddress { get; set; }
        public double X { get; set; }
        public double Y { get; set; }

        public UspsDeliveryPointLink(int zipCode, string gridName, int weight, string name, double x, double y) :
            base(zipCode, gridName, weight)
        {
            MatchAddress = name;
            X = x;
            Y = y;
        }
    }
}
