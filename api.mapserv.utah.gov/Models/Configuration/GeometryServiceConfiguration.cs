namespace api.mapserv.utah.gov.Models.Configuration {
    public class GeometryServiceConfiguration {
        // http://localhost:6080/arcgis/rest/services/Geometry/GeometryServer/project?f=json&amp;{0}
        private const string Template = "{Host}/{ServiceLocation}/project{0}";
        public string Protocol { get; set; } = "http";
        public string Host { get; set; } = "localhost";
        public string Port { get; set; } = "80";
        public string Path { get; set; } = "/arcgis/rest/services/Geometry/GeometryServer/";

        public string GetHost() {
            var host = Host;

            if (Port != "80") {
                host = $"{host}:{Port}";
            }

            return $"{Protocol}://{host}";
        }

        public override string ToString() {
            var host = GetHost();

            return Template.Replace("{ServiceLocation}", Path).Replace("//", "/").Replace("{Host}", host);
        }
    }
}
