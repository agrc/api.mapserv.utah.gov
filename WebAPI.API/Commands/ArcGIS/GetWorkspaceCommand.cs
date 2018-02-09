using System;
using System.Configuration;
using ESRI.ArcGIS;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using Serilog;
using WebAPI.API.Commands.Geocode;
using WebAPI.Common.Abstractions;
using WebAPI.Common.Executors;

namespace WebAPI.API.Commands.ArcGIS
{
    public class GetWorkspaceCommand : Command<IWorkspace>
    {
        private readonly string _connectionString;
        private static readonly string NotifyEmails = ConfigurationManager.AppSettings["notify_email"];

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
            var licenseInitializer = new LicenseInitializer();
            if (!licenseInitializer.InitializeApplication(new[] { esriLicenseProductCode.esriLicenseProductCodeArcServer },
                new esriLicenseExtensionCode[] { }))
            {
                Log.Fatal("Could not authorize product. {@licenseTHing}", licenseInitializer);
                return;
            }

            var factoryType = Type.GetTypeFromProgID("esriDataSourcesGDB.SdeWorkspaceFactory");
            var workspaceFactory2 = (IWorkspaceFactory2)Activator.CreateInstance(factoryType);

            Result = workspaceFactory2.OpenFromFile(_connectionString, 0);
        }
 }


    internal partial class LicenseInitializer
    {
        public LicenseInitializer()
        {
            ResolveBindingEvent += BindingArcGISRuntime;
        }

        void BindingArcGISRuntime(object sender, EventArgs e)
        {
            // TODO: Modify ArcGIS runtime binding code as needed
            if (RuntimeManager.Bind(ProductCode.Desktop))
            {
                return;
            }

            // Failed to bind, announce and force exit
            Console.WriteLine("Invalid ArcGIS runtime binding. Application will shut down.");
            Environment.Exit(0);
        }
    }
}