using ESRI.ArcGIS.Geometry;

namespace WSUT.Upgrade.Soe.Models
{
    public class PointInPolyArgs
    {
        public PointInPolyArgs(string layerName, double utmx, double utmy, string[] attributeList)
        {
            LayerName = layerName;
            Point = new Point
                    {
                        X = utmx,
                        Y = utmy
                    };

            AttributeList = attributeList;
        }

        public string LayerName { get; set; }
        public Point Point { get; set; }
        public string[] AttributeList { get; set; }
    }
}