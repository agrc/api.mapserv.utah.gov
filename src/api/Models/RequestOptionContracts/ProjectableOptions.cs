using System.ComponentModel;

#nullable enable
namespace AGRC.api.Models.RequestOptionContracts;
public class ProjectableOptions : OptionBase {
    /// <summary>
    /// The spatial reference defines how the coordinates will represent a location on the earth defined by how the
    /// round earth was made flat. The well known id's (WKID) of different coordinate systems define if the
    /// coordinates will be stored as degrees of longitude and latitude, meters, feet, etc. This endpoint supports
    /// the WKIDs from the
    /// [Geographic](https://desktop.arcgis.com/en/arcmap/10.5/map/projections/pdf/geographic_coordinate_systems.pdf)
    /// and
    /// [Projected](https://desktop.arcgis.com/en/arcmap/10.5/map/projections/pdf/projected_coordinate_systems.pdf)
    /// coordinate systems. UTM Zone 12 N, with the WKID of **26912**, is the default.
    /// This coordinate system is the most accurate reflection of Utah. It is recommended to use this coordinate
    /// system if length and area calculations are important as other coordinate systems will skew the truth.
    /// </summary>
    [DefaultValue(26912)]
    public int SpatialReference { get; set; } = 26912;
}
