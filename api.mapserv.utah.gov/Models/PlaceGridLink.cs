namespace api.mapserv.utah.gov.Models
{
  public class PlaceGridLink : GridLinkable
  {
      public PlaceGridLink(string city, string grid, int weight) :
          base(grid, weight)
      {
          City = city;
      }

      public string City { get; set; }

      public override string Key => City.ToLowerInvariant();
  }
}
