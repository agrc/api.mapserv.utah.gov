using System.Globalization;

namespace api.mapserv.utah.gov.Models
{
  public class ZipGridLink : GridLinkable
  {
      public ZipGridLink(int zipCode, string gridName, int weight) :
          base(gridName, weight)
      {
          ZipCode = zipCode;
      }


      public int ZipCode { get; set; }

      public override string Key => ZipCode.ToString(CultureInfo.InvariantCulture);
  }
}
