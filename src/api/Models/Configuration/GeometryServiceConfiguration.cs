namespace AGRC.api.Models.Configuration {
    public class GeometryServiceConfiguration : GisServerConfigurationBase {
        // http://localhost:6080/arcgis/rest/services/Geometry/GeometryServer/project?f=json&amp;{0}
        private const string Template = "{Host}/{ServiceLocation}/project{0}";
        public string Path { get; set; } = "/arcgis/rest/services/Geometry/GeometryServer/";

        public override string Url() {
            var host = base.GetHost();

            return Template.Replace("{ServiceLocation}", Path).Replace("//", "/").Replace("{Host}", host);
        }
    }
}
