using System;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Soe.Common.GDB.Connect
{
    public class Sgid93ConnectionInformation : IConnectionInformation
    {
        private readonly string _name;
        private readonly string _password;
        private readonly string _instance;

        public Sgid93ConnectionInformation()
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
                    else if (node.Attributes["key"].Value == "sgid_instance")
                    {
                        _instance = "sde:sqlserver:" + node.Attributes["value"].Value;
                    }
                }
            }
        }

        public string DatabaseName
        {
            get { return "SGID93"; }
        }

        public string InstanceName
        {
            get { return _instance; }
        }

        public string Name
        {
            get { return _name; }
        }

        public string Password
        {
            get{ return _password; }
        }

        public string Version
        {
            get { return "sde.DEFAULT"; }
        }
    }
}