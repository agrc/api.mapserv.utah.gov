namespace api.mapserv.utah.gov.Models
{
  public class PlaceGridLink : GridLinkable
  {
      public PlaceGridLink(string place, string grid, int weight) :
          base(grid, weight)
      {
          City = place;
      }

      public string City { get; set; }

      public override string Key => City.ToLowerInvariant();
  }
}
