using System.Linq;
using ESRI.ArcGIS.Geodatabase;
using Soe.Common.Infastructure.Commands;
using WSUT.Upgrade.Soe.Models;

namespace Wsut.Upgrade.Soe.Commands
{
    public class PointInPolygoinQueryCommand : Command<FieldValueMap[]>
    {
        public PointInPolygoinQueryCommand(PointInPolyArgs searchArgs, IFeatureWorkspace workspace)
        {
            SearchArgs = searchArgs;
            Workspace = workspace;
        }

        public PointInPolyArgs SearchArgs { get; set; }
        public IFeatureWorkspace Workspace { get; set; }

        protected override void Execute()
        {
            var featureClass = Workspace.OpenFeatureClass(SearchArgs.LayerName.Trim());

            var spatFilter = new SpatialFilter
                {
                    SpatialRel = esriSpatialRelEnum.esriSpatialRelWithin,
                    Geometry = SearchArgs.Point
                };

            var fCursor = featureClass.Search(spatFilter, true);

            var fields = featureClass.Fields;
            var feature = fCursor.NextFeature();

            if (feature == null)
            {
                Result = null;

                return;
            }

            Result =
                SearchArgs.AttributeList.Select(
                    x =>
                    new FieldValueMap(x.Trim(),
                                      CommandExecutor.ExecuteCommand(new GetValueForFieldCommand(x, fields, feature))))
                .ToArray();
        }

        public override string ToString()
        {
            return string.Format("{0}, SearchArgs: {1}", "PointInPolygoinQueryCommand", SearchArgs);
        }
    }
}