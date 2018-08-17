using System.Globalization;

namespace api.mapserv.utah.gov.Models.Linkables {
    public class ZipGridLink : GridLinkable {
        public ZipGridLink(int zip, string grid, int weight) :
            base(grid, weight) {
            ZipCode = zip;
        }

        public int ZipCode { get; set; }

        public override string Key => ZipCode.ToString(CultureInfo.InvariantCulture);
    }
}
