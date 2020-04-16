using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class UrezPhase1WindZones
    {
        public int Xid { get; set; }
        public decimal? SiteId { get; set; }
        public string Sitename { get; set; }
        public string Counties { get; set; }
        public string MwPotent { get; set; }
        public string MwEsttech { get; set; }
        public decimal? Estspeed80m { get; set; }
        public decimal? Gcf101dens { get; set; }
        public decimal? ElevFt { get; set; }
        public decimal? Ge15grscap { get; set; }
        public string Comments { get; set; }
        public string Confidence { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
