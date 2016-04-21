using System.Linq;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using Soe.Common.Infastructure.Commands;
using WebAPI.Search.Soe.Models;
using WebAPI.Search.Soe.Models.Results;

namespace WebAPI.Search.Soe.Commands.Factory
{
    public static class SpatialCommandFactory
    {
        public static Command<ResponseContainer<SearchResult>>  Get(GeometryContainer container,
                                                                   IFeatureWorkspace featureWorkspace,
                                                                   string featureClass, string[] values,
                                                                   string predicate,
                                                                   ISpatialReference newSpatialRefefence)
        {
            if (container == null)
                return new NonSpatialQueryCommand(featureWorkspace,
                                                  new QueryArgs(featureClass, values, predicate, newSpatialRefefence));

            var rasterDatasets = new[] { "RASTER.DEM_10METER" };

            if (rasterDatasets.Any(x => featureClass.ToUpperInvariant().Contains(x)))
            {
                var rasterWorkspace = featureWorkspace as IRasterWorkspaceEx;
                return new PointInRasterQueryCommand(rasterWorkspace,
                                                     new SpatialQueryArgs(featureClass, container.Geometry,
                                                                          values, predicate, newSpatialRefefence));
            }

            switch (container.Type)
            {
                case "POINT":
                case "POLYGON":
                    {
                        return new PointInPolygonQueryCommand(featureWorkspace,
                                                              new SpatialQueryArgs(featureClass, container.Geometry,
                                                                                   values, predicate,
                                                                                   newSpatialRefefence));
                    }
                default:
                    {
                        return new NonSpatialQueryCommand(featureWorkspace,
                                                          new QueryArgs(featureClass, values, predicate,
                                                                        newSpatialRefefence));
                    }
            }
        }
    }
}