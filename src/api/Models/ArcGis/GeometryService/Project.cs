namespace ugrc.api.Models.ArcGis;
public static class Project {
    public class RequestContract : RequestContractBase<RequestGeometry> {
        /// <summary>
        ///
        /// </summary>
        public int OutSr { get; set; } = 26912;
        public int InSr { get; set; } = 26912;
        public string? QueryString {
            get {
                var query = new QueryString("?f=json")
                    .Add("geometries", Locations[0].ToString())
                    .Add("outSR", OutSr.ToString())
                    .Add("inSR", InSr.ToString());

                return query.Value;
            }
        }
    }

    public record RequestGeometry(Point? Geometry) {
        public override string ToString()
            => $$$"""{"geometryType":"esriGeometryPoint","geometries":[{"x":{{{Geometry?.X}}},"y":{{{Geometry?.Y}}}}]}""";
    }

    public record ResponseContract(IReadOnlyCollection<Point> Geometries, RestEndpointError? Error)
        : RestErrorable(Error);
}
