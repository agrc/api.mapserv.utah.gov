using System.Runtime.InteropServices;
using ESRI.ArcGIS.SOESupport;
using ESRI.ArcGIS.Server;
using ESRI.ArcGIS.esriSystem;
using Soe.Common.Infastructure.Commands;
using Soe.Common.Infastructure.IOC;
using Wsut.Upgrade.Soe.Commands;

//TODO: sign the project (project properties > signing tab > sign the assembly)
//      this is strongly suggested if the dll will be registered using regasm.exe <your>.dll /codebase

namespace WSUT.Upgrade.Soe
{
    [ComVisible(true)]
    [Guid("0a883de7-87ab-4e1e-ae84-cd2eeba8ebfc")]
    [ClassInterface(ClassInterfaceType.None)]
    [ServerObjectExtension("MapServer",
        //use "MapServer" if SOE extends a Map service and "ImageServer" if it extends an Image service.
        AllCapabilities = "",
        DefaultCapabilities = "",
        Description = "Allows all WSUT legacy services to continue working.",
        DisplayName = "WSUT.Upgrade",
        Properties = "",
        SupportsREST = true,
        SupportsSOAP = false)]
    public class WsutUpgradeSoe : SoeBase, IServerObjectExtension, IObjectConstruct, IRESTRequestHandler
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="WsutUpgradeSoe" /> class. If you have business logic that you want to
        ///     run when the SOE first becomes enabled, don’t here; instead, use the following IObjectConstruct.Construct() method
        ///     found in SoeBase.cs
        /// </summary>
        public WsutUpgradeSoe()
        {
            ReqHandler = CommandExecutor.ExecuteCommand(
                new CreateRestImplementationCommand(typeof (FindAllEndpointsCommand).Assembly));
        }

        private Container Kernel { get; set; }
    }
}