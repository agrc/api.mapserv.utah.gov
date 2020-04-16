using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class OilGasWells
    {
        public int Xid { get; set; }
        public string Api { get; set; }
        public string Wellname { get; set; }
        public string Operator { get; set; }
        public decimal? Operatorno { get; set; }
        public string Fieldname { get; set; }
        public decimal? GroundElev { get; set; }
        public decimal? KellyElev { get; set; }
        public decimal? DrkfloorElev { get; set; }
        public decimal? CoordssurfN { get; set; }
        public decimal? CoordssurfE { get; set; }
        public string Utmzone { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public decimal? Footagens { get; set; }
        public string DirNs { get; set; }
        public decimal? Footageew { get; set; }
        public string DirEw { get; set; }
        public string Qtrqtr { get; set; }
        public decimal? Section { get; set; }
        public decimal? Township { get; set; }
        public string Townshipdir { get; set; }
        public decimal? Range { get; set; }
        public string Rangedir { get; set; }
        public string Meridian { get; set; }
        public string County { get; set; }
        public string DirHoriz { get; set; }
        public string DirVert { get; set; }
        public string DirDirect { get; set; }
        public string Confidential { get; set; }
        public DateTime? Confreldate { get; set; }
        public string Leasenumber { get; set; }
        public string Leasetype { get; set; }
        public string Surfaceowner { get; set; }
        public DateTime? Abandondate { get; set; }
        public string Wellstatus { get; set; }
        public string Welltype { get; set; }
        public decimal? TotcumOil { get; set; }
        public decimal? TotcumGas { get; set; }
        public decimal? TotcumWater { get; set; }
        public string Indiantribe { get; set; }
        public decimal? MultiLats { get; set; }
        public string Originalfieldtype { get; set; }
        public string Unitname { get; set; }
        public string Gisstatustype { get; set; }
        public DateTime? Origcompldate { get; set; }
        public string Jurisdiction { get; set; }
        public Point Shape { get; set; }
    }
}
