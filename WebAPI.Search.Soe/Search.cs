#region License

// 
// Copyright (C) 2012 AGRC
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software 
// and associated documentation files (the "Software"), to deal in the Software without restriction, 
// including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
// and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, 
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial 
// portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT
// NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
// SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 

#endregion

using System.Runtime.InteropServices;
using ESRI.ArcGIS.SOESupport;
using ESRI.ArcGIS.Server;
using ESRI.ArcGIS.esriSystem;
using Soe.Common.Infastructure.Commands;
using Soe.Common.Infastructure.IOC;
using WebAPI.Search.Soe.Commands;

namespace WebAPI.Search.Soe
{
    /// <summary>
    ///   The main server object extension
    /// </summary>
    [ComVisible(true)]
    [Guid("021ED22C-4D23-4A84-9CC3-A16D24BA1D5E")]
    [ClassInterface(ClassInterfaceType.None)]
    [ServerObjectExtension("MapServer",
        AllCapabilities = "",
        //These create checkboxes to determine allowed functionality
        DefaultCapabilities = "",
        Description = "Allows web api to send requests for spatial operations.",
        //shows up in manager under capabilities
        DisplayName = "WebAPI.Searching",
        //Properties that can be set on the capabilities tab in manager.
        Properties = "",
        SupportsREST = true,
        SupportsSOAP = false)]
    public class Search : SoeBase, IServerObjectExtension, IObjectConstruct, IRESTRequestHandler
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="Search" /> class. If you have business logic that you want to run when the SOE first becomes enabled, don’t here; instead, use the following IObjectConstruct.Construct() method found in SoeBase.cs
        /// </summary>
        public Search()
        {
            ReqHandler = CommandExecutor.ExecuteCommand(
                new CreateRestImplementationCommand(typeof (SoeBase).Assembly));

            
            Kernel = new Container();
        }

        private Container Kernel { get; set; }


    }
}