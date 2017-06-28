using System;
using ESRI.ArcGIS;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using WebAPI.Common.Abstractions;

namespace WebAPI.API.Commands.ArcGIS
{
    public class GetWorkspaceCommand : Command<IWorkspace>
    {
        private readonly string _connectionString;

        public GetWorkspaceCommand(string connectionString)
        {
            _connectionString = connectionString;
        }

        public override string ToString()
        {
            return "GetWorkspaceCommand";
        }

        protected override void Execute()
        {
            RuntimeManager.Bind(ProductCode.Server);

            var init = new AoInitialize();
            init.Initialize(esriLicenseProductCode.esriLicenseProductCodeBasic);

            var factoryType = Type.GetTypeFromProgID("esriDataSourcesGDB.SdeWorkspaceFactory");
            var workspaceFactory2 = (IWorkspaceFactory2)Activator.CreateInstance(factoryType);

            Result = workspaceFactory2.OpenFromFile(_connectionString, 0);
        }
    }
}