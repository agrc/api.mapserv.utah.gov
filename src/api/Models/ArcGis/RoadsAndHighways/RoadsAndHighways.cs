namespace AGRC.api.Models.ArcGis {
    public class SpatialReference {
        public int WKID { get; set; }
    }

    public enum GeometryType {
        esriGeometryPoint,
        esriGeometryPolyline,
        esriGeometryPolygon,
    }
}
