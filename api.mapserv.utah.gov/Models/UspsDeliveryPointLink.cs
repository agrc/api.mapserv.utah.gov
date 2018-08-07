using System;

namespace api.mapserv.utah.gov.Models
{
    public class UspsDeliveryPointLink : ZipGridLink
    {
        public string MatchAddress { get; set; }
        public double X { get; set; }
        public double Y { get; set; }

        public UspsDeliveryPointLink(int zip, string grid, string place, decimal x, decimal y) :
            base(zip, grid, 0)
        {
            MatchAddress = place;
            X = Convert.ToDouble(x);
            Y = Convert.ToDouble(y);
        }
    }
}
