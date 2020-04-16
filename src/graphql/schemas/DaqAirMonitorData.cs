using System;
using System.Collections.Generic;

namespace graphql.schemas
{
    public partial class DaqAirMonitorData
    {
        public int Xid { get; set; }
        public string Sitename { get; set; }
        public string Sitedescription { get; set; }
        public string Maplabel { get; set; }
        public string Epaid { get; set; }
        public string County { get; set; }
        public string City { get; set; }
        public string Streetaddress1 { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string Parametername { get; set; }
        public string Aqsparameterdescription { get; set; }
        public decimal? Finalvalue { get; set; }
        public string Reportedunitname { get; set; }
        public string Nullcode { get; set; }
        public DateTime? Date { get; set; }
    }
}
