using System.Globalization;

namespace WebAPI.Domain.Linkers
{
    public class ZipGridLink : GridLinkable
    {
        public ZipGridLink(int zipCode, string gridName, int weight) : 
            base(gridName,weight)
        {
            ZipCode = zipCode;
        }


        public int ZipCode { get; set; }

        public override string Key
        {
            get { return ZipCode.ToString(CultureInfo.InvariantCulture); }
        }
    }
}