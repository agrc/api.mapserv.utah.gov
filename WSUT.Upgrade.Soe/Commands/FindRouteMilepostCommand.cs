using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using Soe.Common.Infastructure.Commands;
using WSUT.Upgrade.Soe.Models;

namespace Wsut.Upgrade.Soe.Commands
{
    public class FindRouteMilepostCommand : Command<GeocodeResult>
    {
        public FindRouteMilepostCommand(string milePost, string type, string routeNumber,
                                        string sgid10TransportationUdotroutesLrs, IFeatureWorkspace workspace)
        {
            MilePost = milePost;
            Type = type;
            RouteNumber = routeNumber;
            LayerName = sgid10TransportationUdotroutesLrs;
            Workspace = workspace;
        }

        protected string MilePost { get; set; }

        protected string Type { get; set; }

        protected string RouteNumber { get; set; }

        public string LayerName { get; set; }

        public IFeatureWorkspace Workspace { get; set; }

        protected override void Execute()
        {
            Result = null;
            var milepostInt = double.Parse(MilePost);
            var featureClass = Workspace.OpenFeatureClass(LayerName);

            var filter = new QueryFilter
                         {
                             WhereClause =
                                 string.Format("RT_NAME = '{0}' AND RT_DIR = '{1}'", RouteNumber.ToUpper(), Type)
                         };

            var cursor = featureClass.Search(filter, false);

            IFeature feature;

            while((feature  = cursor.NextFeature()) != null)
            {
                var segmentation = feature.Shape as IMSegmentation3;
                if (segmentation == null)
                {
                    continue;
                }

                var collection = segmentation.GetPointsAtM(milepostInt, 0);
                if (collection.GeometryCount <= 0)
                {
                    continue;
                }

                var point = collection.Geometry[0] as IPoint;

                if (point == null)
                {
                    continue;
                }

                var result = new GeocodeResult(string.Format("Route {0}, Milepost {1}", RouteNumber, MilePost),
                                           LayerName, 100)
                {
                    UTM_X = point.X,
                    UTM_Y = point.Y
                };

                var latLong = ConvertUtmToLatLong(point);

                result.LONG_X = latLong[0];
                result.LAT_Y = latLong[1];

                Result = result;
                
                return;
            }            
        }

        private static double[] ConvertUtmToLatLong(IPoint utmPoint)
        {
            var spatFac = new SpatialReferenceEnvironment();
            var refWgs1984 = spatFac.CreateGeographicCoordinateSystem(4326);

            utmPoint.Project(refWgs1984);

            return new[]
                   {
                       utmPoint.X, utmPoint.Y
                   };
        }

        public override string ToString()
        {
            return string.Format("{0}, MilePost: {1}, Type: {2}, RouteNumber: {3}", "FindRouteMilepostCommand", MilePost,
                                 Type, RouteNumber);
        }
    }
}