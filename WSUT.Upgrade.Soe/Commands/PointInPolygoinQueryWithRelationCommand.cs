using System.Collections.Generic;
using System.Linq;
using ESRI.ArcGIS.Geodatabase;
using Soe.Common.Infastructure.Commands;
using WSUT.Upgrade.Soe.Models;

namespace Wsut.Upgrade.Soe.Commands
{
    internal class PointInPolygoinQueryWithRelationCommand : Command<FieldValueMap[]>
    {
        public PointInPolygoinQueryWithRelationCommand(PointInPolyArgsWithRelation searchArgs,
                                                       IFeatureWorkspace workspace)
        {
            SearchArgs = searchArgs;
            Workspace = workspace;
            Values = new List<FieldValueMap>();
        }

        public PointInPolyArgsWithRelation SearchArgs { get; set; }
        public IFeatureWorkspace Workspace { get; set; }
        private List<FieldValueMap> Values { get; set; }

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

            while (feature != null)
            {
                var relatedClass = Workspace.OpenRelationshipClass(SearchArgs.RelationLayerName.Trim());
                var relatedObjects = relatedClass.GetObjectsRelatedToObject(feature);

                var relatedObject = relatedObjects.Next();

                if (relatedObject == null)
                {
                    continue;
                }

                while (relatedObject != null)
                {
                    var iobject = relatedObject as IObject;

                    if (iobject == null)
                    {
                        continue;
                    }

                    Values.AddRange(SearchArgs.RelatedAttributeList.Select(x => new FieldValueMap(
                                                                                    x.Trim(),
                                                                                    CommandExecutor.ExecuteCommand(
                                                                                        new GetValueForFieldCommand(x,
                                                                                                                    iobject
                                                                                                                        .
                                                                                                                        Fields,
                                                                                                                    iobject)))));
                    relatedObject = relatedObjects.Next();
                }

                Values.AddRange(SearchArgs.AttributeList.Select(x => new FieldValueMap
                                                                         (x.Trim(),
                                                                          CommandExecutor.ExecuteCommand(
                                                                              new GetValueForFieldCommand(x, fields,
                                                                                                          feature)))));
                feature = fCursor.NextFeature();
            }

            Result = Values.ToArray();
        }

        public override string ToString()
        {
            return string.Format("{0}, SearchArgs: {1}", "PointInPolygoinQueryWithRelationCommand", SearchArgs);
        }
    }
}