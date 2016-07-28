using System;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Soe.Common.GDB.Connect
{
    public class Sgid10ConnectionInformation : IConnectionInformation
    {
        private readonly string _name;
        private readonly string _password;
        private readonly string _server;

        public Sgid10ConnectionInformation()
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
                    if (node.Attributes["key"].Value == "sde_search_user")
                    {
                        _name = node.Attributes["value"].Value;
                    }
                    else if (node.Attributes["key"].Value == "sde_search_password")
                    {
                        _password = node.Attributes["value"].Value;
                    }
                    else if (node.Attributes["key"].Value == "sgid_server")
                    {
                        _server = node.Attributes["value"].Value;
                    }
                }
            }
        }

        public string DatabaseName
        {
            get { return "SGID10"; }
        }

        public string InstanceName
        {
            get { return "sde:sqlserver:" + _server; }
        }

        public string Name
        {
            get { return _name; }
        }

        public string Password
        {
            get { return _password; }
        }

        public string ServerName
        {
            get { return _server; }
        }

        public string Version
        {
            get { return "sde.DEFAULT"; }
        }
    }
}