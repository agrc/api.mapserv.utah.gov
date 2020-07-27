using System.ComponentModel;

namespace AGRC.api.Models.RequestOptionContracts {
    public class ProjectableOptions : OptionBase {
        /// <summary>
        ///     The spatial reference for the output and input geometries.
        ///     Choose any of the <abbr title="Well-known Id">wkid</abbr>'s from the
        ///     <a href="http://resources.arcgis.com/en/help/main/10.1/018z/pdf/geographic_coordinate_systems.pdf">
        ///         Geographic
        ///         Coordinate System wkid reference
        ///     </a>
        ///     or
        ///     <a href="http://resources.arcgis.com/en/help/main/10.1/018z/pdf/projected_coordinate_systems.pdf">
        ///         Projected
        ///         Coordinate System wkid reference
        ///     </a>
        ///     .
        ///     Default: 26912
        /// </summary>
        [DefaultValue(26912)]
        public int SpatialReference { get; set; } = 26912;
    }
}
