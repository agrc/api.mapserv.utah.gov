using System;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Soe.Common.GDB.Connect
{
    public class SgidConnectionStringFactory
    {
        internal static class ConnectionStringBuilder
        {
            private static string _instance;

            public static string Create(IConnectionInformation connectionInfo)
            {
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Soe.Common.App.config"))
                {
                    if (stream == Stream.Null)
                    {
                        throw new ArgumentNullException("stream");
                    }

                    var xdoc = new XmlDocument();
                    xdoc.Load(stream);

                    var xnodes = xdoc.SelectSingleNode("/configuration/appSettings");
                    foreach (XmlNode node in xnodes.ChildNodes)
                    {
                        if (node.Attributes["key"].Value == "sgid_instance")
                        {
                            _instance = node.Attributes["value"].Value;
                        }
                    }
                }

                return string.Format("Data Source={3};Initial Catalog={0};Persist Security Info=True;User ID={1};Password={2}", 
                                     connectionInfo.DatabaseName,connectionInfo.Name,connectionInfo.Password, _instance);
            }
        }

        public static string Create(string layerName)
        {
            if (layerName.ToUpperInvariant().StartsWith("SGID93"))
                return ConnectionStringBuilder.Create(new Sgid93ConnectionInformation());

            if (layerName.ToUpperInvariant().StartsWith("SGID10"))
                return ConnectionStringBuilder.Create(new Sgid10ConnectionInformation());

            return null;
        }
    }
}