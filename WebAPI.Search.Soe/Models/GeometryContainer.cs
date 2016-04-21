using System.Collections.Generic;
using ESRI.ArcGIS.Geometry;

namespace WebAPI.Search.Soe.Models
{
    public class GeometryContainer
    {
        public ICollection<double[]> Coordinates { get; set; }
        public string Type { get; set; }
        public IGeometry Geometry { get; set; }
    }
}