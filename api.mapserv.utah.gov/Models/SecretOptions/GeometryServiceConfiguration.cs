namespace api.mapserv.utah.gov.Models.SecretOptions
{
    public class GeometryServiceConfiguration
    {
        // http://localhost:6080/arcgis/rest/services/Geometry/GeometryServer/project?f=json&amp;{0}
        private const string _template = "{Host}/{ServiceLocation}/project?f=json&{0}";
        public string Host { get; set; } = "localhost";
        public string Port { get; set; } = "80";
        public string Path { get; set; } = "/arcgis/rest/services/Geometry/GeometryServer/";

        public string ProjectUrl
        {
            get
            {
                if (Port != "80")
                {
                    Host = Host + ":" + Port;
                }

                return _template.Replace("{Host}", Host).Replace("{ServiceLocation}", Path).Replace("//", "/");
            }
        }
    }
}
