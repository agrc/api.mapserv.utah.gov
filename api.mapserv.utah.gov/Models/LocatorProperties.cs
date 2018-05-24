namespace api.mapserv.utah.gov.Models
{
    public class LocatorProperties
    {
        public string Url { get; set; }
        public string Name { get; set; }
        public int Weight { get; set; }

        public override string ToString() => $"LocatorDetails - Name: {Name}, Url: {Url}, Weight: {Weight}";
    }
}
