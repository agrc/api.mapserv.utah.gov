using System;
using ESRI.ArcGIS;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using Serilog;
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
            if (!RuntimeManager.Bind(ProductCode.Server) && !RuntimeManager.Bind(ProductCode.EngineOrDesktop))
            {
                return;
            }

            var licenseInitializer = new LicenseInitializer();
            if (!licenseInitializer.InitializeApplication(
                new[] {esriLicenseProductCode.esriLicenseProductCodeArcServer},
                new esriLicenseExtensionCode[] { }))
            {
                Log.Fatal("Could not authorize product. {@licenseThing}", licenseInitializer);
                licenseInitializer.ShutdownApplication();
            }

            var factoryType = Type.GetTypeFromProgID("esriDataSourcesGDB.SdeWorkspaceFactory");
            var workspaceFactory2 = (IWorkspaceFactory2) Activator.CreateInstance(factoryType);

            Result = workspaceFactory2.OpenFromFile(_connectionString, 0);
        }
    }
}