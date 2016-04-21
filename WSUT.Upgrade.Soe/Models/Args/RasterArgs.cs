namespace WSUT.Upgrade.Soe.Models.Args
{
    public class RasterArgs
    {
        public RasterArgs(string layerName, double? utmx, double? utmy)
        {
            LayerName = layerName;
            X = utmx.GetValueOrDefault(0);
            Y = utmy.GetValueOrDefault(0);
        }

        public string LayerName { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
    }
}