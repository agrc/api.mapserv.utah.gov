using System.Collections.ObjectModel;
using System.Linq;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using Soe.Common.Infastructure.Commands;
using WSUT.Upgrade.Soe.Models.Args;
using Envelope = WSUT.Upgrade.Soe.Models.Envelope;

namespace Wsut.Upgrade.Soe.Commands
{
    public class GetEnvelopeCommand : Command<Envelope[]>
    {
        public GetEnvelopeCommand(EnvelopeArgs searchArgs, IFeatureWorkspace workspace)
        {
            SearchArgs = searchArgs;
            Workspace = workspace;
        }

        public EnvelopeArgs SearchArgs { get; set; }
        public IFeatureWorkspace Workspace { get; set; }

        protected override void Execute()
        {
            var featureClass = Workspace.OpenFeatureClass(SearchArgs.LayerName.Trim());

            var spatFilter = new QueryFilter
                             {
                                 WhereClause = SearchArgs.WhereClause
                             };

            var fCursor = featureClass.Search(spatFilter, true);

            var fields = featureClass.Fields;

            var collection = new Collection<Envelope>();

            var feature = fCursor.NextFeature();
            while (feature != null)
            {
                var geom = GetValueAtIndexForField(featureClass.ShapeFieldName, fields, feature) as IGeometry;

                if (geom == null)
                {
                    continue;
                }

                collection.Add(new Envelope(
                                   geom.Envelope.XMin,
                                   geom.Envelope.YMin,
                                   geom.Envelope.XMax,
                                   geom.Envelope.YMax,
                                   ""));

                feature = fCursor.NextFeature();
            }

            if (collection.Count == 0)
            {
                Result = null;
            }

            Result = collection.ToArray();
        }

        private static object GetValueAtIndexForField(string x, IFields fields, IFeature feature)
        {
            var findField = fields.FindField(x.Trim());

            return findField < 0 ? string.Format("Attribute {0} not found.", x) : feature.Value[findField];
        }

        public override string ToString()
        {
            return string.Format("{0}, SearchArgs: {1}", "GetEnvelopeCommand", SearchArgs);
        }
    }
}