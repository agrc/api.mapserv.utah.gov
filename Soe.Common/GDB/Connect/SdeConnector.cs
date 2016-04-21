using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;

namespace Soe.Common.GDB.Connect
{
    public class SdeConnector : Connector
    {
        public SdeConnector(IConnectionInformation information)
            : base(information) {}

        public override IWorkspace Connect()
        {
            var propertySet = new PropertySet();
            propertySet.SetProperty("SERVER", "localhost");
            propertySet.SetProperty("INSTANCE", Information.InstanceName);
            propertySet.SetProperty("DATABASE", Information.DatabaseName);
            propertySet.SetProperty("USER", Information.Name);
            propertySet.SetProperty("PASSWORD", Information.Password);
            propertySet.SetProperty("VERSION", Information.Version);

            var workspaceFactory = new SdeWorkspaceFactory();
            var workspace = workspaceFactory.Open(propertySet, 0);

            return workspace;
        }
    }
}