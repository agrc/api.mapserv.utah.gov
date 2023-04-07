namespace AGRC.api.Models.ArcGis;
public enum GeometryType {
    esriGeometryPoint,
    esriGeometryPolyline,
    esriGeometryPolygon,
}

public record SpatialReference(int WKID);
