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

using System;
using System.Reflection;
using ESRI.ArcGIS.SOESupport;
using Soe.Common.Infastructure.Commands;
using Soe.Common.Infastructure.Endpoints;
using Wsut.Upgrade.Soe.Endpoints;

namespace Wsut.Upgrade.Soe.Commands
{
    /// <summary>
    ///   Creates the SOE rest implementation. Searches the assembly and adds all endpoints denoted with the Endpoint attribute.
    /// </summary>
    public class CreateRestImplementationCommand : Command<SoeRestImpl>
    {
        private const string Restoperation = "RestOperation";
        private readonly Assembly _assemblyToScan;

        /// <summary>
        ///   Initializes a new instance of the <see cref="CreateRestImplementationCommand" /> class.
        /// </summary>
        /// <param name="assembly"> The assembly. </param>
        public CreateRestImplementationCommand(Assembly assembly)
        {
            _assemblyToScan = assembly;
        }

        /// <summary>
        ///   code to execute when command is run. adds a default ResourceHandler and scans assembly for all OperationHandlers marked with the EndpointAttribute
        /// </summary>
        protected override void Execute()
        {
            var resource = new RestResource(typeof (VersionEndpoint).Assembly.FullName, false, VersionEndpoint.Handler);

            foreach (var type in CommandExecutor.ExecuteCommand(
                new FindAllEndpointsCommand(_assemblyToScan)))
            {
                if (!typeof (IRestEndpoint).IsAssignableFrom(type))
                {
                    continue;
                }

                var methodInfo = type.GetMethod(Restoperation);
                var instance = Activator.CreateInstance(type);

                var restOperation = methodInfo.Invoke(instance, null) as RestOperation;

                if (restOperation == null)
                {
                    continue;
                }

                resource.operations.Add(restOperation);
            }

            Result = new SoeRestImpl(Assembly.GetExecutingAssembly().FullName, resource);
        }

        public override string ToString()
        {
            return string.Format("{0}", "CreateRestImplementationCommand");
        }
    }
}