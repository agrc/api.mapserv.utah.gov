using Soe.Common.Extensions;

namespace WSUT.Upgrade.Soe.Models.Args
{
    public class EnvelopeArgs
    {
        public EnvelopeArgs(string layerName, int[] objectid)
        {
            LayerName = layerName;
            ObjectIds = objectid;
        }

        public string LayerName { get; set; }
        public int[] ObjectIds { get; set; }

        public string WhereClause
        {
            get { return "OBJECTID IN ({0})".With(string.Join(",", ObjectIds)); }
        }

        public override string ToString()
        {
            return string.Format("LayerName: {0}, ObjectIds: {1}", LayerName, string.Join(",", ObjectIds));
        }
    }
}
