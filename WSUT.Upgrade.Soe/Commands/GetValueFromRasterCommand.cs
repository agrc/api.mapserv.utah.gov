using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using Soe.Common.Infastructure.Commands;
using WSUT.Upgrade.Soe.Models.Args;

namespace Wsut.Upgrade.Soe.Commands
{
    public class GetValueFromRasterCommand : Command<string>
    {
        public GetValueFromRasterCommand(RasterArgs rasterArgs, IRasterWorkspaceEx rasterWorkspaceEx)
        {
            Args = rasterArgs;
            RasterWorkspace = rasterWorkspaceEx;
        }

        public RasterArgs Args { get; set; }
        public IRasterWorkspaceEx RasterWorkspace { get; set; }

        protected override void Execute()
        {
            var point = new Point
                        {
                            X = Args.X,
                            Y = Args.Y
                        };

            var dataset = RasterWorkspace.OpenRasterDataset(Args.LayerName);

            var rasterLayer = new RasterLayer();

            rasterLayer.CreateFromDataset(dataset);

            var identify = rasterLayer as IIdentify;
            var results = identify.Identify(point);

            if (results == null)
            {
                Result = null;
                return;
            }

            var rasterObj = results.Element[0] as IRasterIdentifyObj;

            if (rasterObj == null)
            {
                Result = null;
                return;
            }

            Result = rasterObj.Name;
        }

        public override string ToString()
        {
            return string.Format("{0}, LayerName: {1}, X: {2}, Y: {3}", "GetValueFromRasterCommands", Args.LayerName,
                                 Args.X, Args.Y);
        }
    }
}