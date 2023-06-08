namespace AGRC.api.Models.ArcGis;
public static class ImageServiceIdentify {
    public record RequestContract(PointWithSpatialReference Geometry, GeometryType GeometryType) {
        public string ToQuery() =>
            $"?geometry={Geometry.ToEsriJson()}&geometryType={GeometryType}&returnGeometry=false&returnCatalogItems=false&f=json";
    }

    public record ResponseContract(string Value, RestEndpointError? Error) : RestErrorable(Error) {
        public string Feet {
            get {
                double meters;
                try {
                    meters = Convert.ToDouble(Value);
                } catch (Exception) {
                    return "NoData";
                }

                return (meters * 3.28084).ToString();
            }
        }
    }
}
