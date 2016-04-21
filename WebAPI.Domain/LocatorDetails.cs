namespace WebAPI.Domain
{
    /// <summary>
    ///     Basic information about the geocoding service
    /// </summary>
    public class LocatorDetails
    {
        public string Url { get; set; }
        public string Name { get; set; }
        public int Weight { get; set; }

        public override string ToString()
        {
            return string.Format("LocatorDetails - Name: {0}, Url: {1}, Weight: {2}", Name, Url, Weight);
        }
    }
}