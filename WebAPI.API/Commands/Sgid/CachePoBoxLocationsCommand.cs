using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using WebAPI.API.Commands.ArcGIS;
using WebAPI.Common.Abstractions;
using WebAPI.Common.Executors;
using WebAPI.Domain;
using Path = System.IO.Path;

namespace WebAPI.API.Commands.Sgid
{
    public class CachePoBoxLocationsCommand : Command<Dictionary<int, PoBoxAddress>>
    {
        public override string ToString()
        {
            return "CachePoBoxLocationsCommand";
        }

        protected override void Execute()
        {
            var poboxes = new Dictionary<int, PoBoxAddress>();
            var file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sgid10.sde");

            var workspace = CommandExecutor.ExecuteCommand(new GetWorkspaceCommand(file));
            var featureWorkspace = (IFeatureWorkspace) workspace;

            var zipCodePoBoxes = featureWorkspace.OpenFeatureClass("LOCATION.ZipCodePOBoxes");
            var postOffices = featureWorkspace.OpenFeatureClass("SOCIETY.PostOffices");

            GetZipLocations(zipCodePoBoxes, "ZIP5", ref poboxes);
            GetZipLocations(postOffices, "ZIP", ref poboxes);

            Result = poboxes;

            Marshal.ReleaseComObject(zipCodePoBoxes);
            Marshal.ReleaseComObject(postOffices);
            Marshal.ReleaseComObject(featureWorkspace);
            Marshal.ReleaseComObject(workspace);
        }

        private static void GetZipLocations(IFeatureClass fc, string field, ref Dictionary<int, PoBoxAddress> poboxes)
        {
            var fields = fc.Fields;
            var index = fields.FindField(field);
            var all = new QueryFilter
            {
                WhereClause = "1=1"
            };

            var cursor = fc.Search(all, true);

            IFeature feature;
            while ((feature = cursor.NextFeature()) != null)
            {
                var data = feature.Value[index];
                if (data == null)
                    continue;

                var zip5 = Convert.ToInt32(data);

                if (zip5 < 8000 || poboxes.ContainsKey(zip5))
                    continue;

                var point = (IPoint) feature.Shape;

                poboxes[zip5] = new PoBoxAddress(zip5, point.X, point.Y);
            }

            Marshal.ReleaseComObject(cursor);
            Marshal.ReleaseComObject(fc);
        }
    }
}