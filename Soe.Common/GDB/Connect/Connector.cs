using ESRI.ArcGIS.Geodatabase;

namespace Soe.Common.GDB.Connect
{
    public abstract class Connector : IConnectable
    {
        internal readonly IConnectionInformation Information;

        protected Connector(IConnectionInformation information)
        {
            Information = information;
        }

        public abstract IWorkspace Connect();
    }
}