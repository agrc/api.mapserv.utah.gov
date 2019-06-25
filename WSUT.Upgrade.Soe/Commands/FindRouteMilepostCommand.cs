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
            WhereClause = $"RT_NAME = '{RouteNumber.ToUpper()}' AND RT_DIR = '{Type}'";
        }

        public FindRouteMilepostCommand(string route, string milePost, string sgid10TransportationUdotroutesLrs, IFeatureWorkspace workspace)
        {
            MilePost = milePost;
            LayerName = sgid10TransportationUdotroutesLrs;
            Workspace = workspace;
            RouteNumber = route.ToUpper();
            WhereClause = $"LABEL='{RouteNumber}'";
            fullRoute = true;
        }

        private bool fullRoute;

        protected string MilePost { get; set; }

        protected string Type { get; set; }

        protected string RouteNumber { get; set; }

        public string LayerName { get; set; }

        private string WhereClause { get; }

        public IFeatureWorkspace Workspace { get; set; }

        protected override void Execute()
        {
            Result = null;
            var milepostInt = double.Parse(MilePost);
            var featureClass = Workspace.OpenFeatureClass(LayerName);

            var filter = new QueryFilter
                         {
                             WhereClause = WhereClause
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

                var result = new GeocodeResult($"Route {RouteNumber}, Milepost {MilePost}",
                                           LayerName, 100, fullRoute)
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
            return $"FindRouteMilepostCommand, MilePost: {MilePost}, Type: {Type}, RouteNumber: {RouteNumber}";
        }
    }
}
