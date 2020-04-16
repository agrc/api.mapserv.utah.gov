using System;
using System.Collections.Generic;

namespace graphql.schemas
{
    public partial class DeqmapTankust
    {
        public int Xid { get; set; }
        public decimal? Facilityid { get; set; }
        public decimal? Tankid { get; set; }
        public string Alttankid { get; set; }
        public string Tankstatus { get; set; }
        public string Federalreg { get; set; }
        public string Tankemerge { get; set; }
        public decimal? Tankcapaci { get; set; }
        public string Substanced { get; set; }
        public DateTime? Datelastus { get; set; }
        public DateTime? Dateclose { get; set; }
        public string Closuresta { get; set; }
        public DateTime? Dateinstal { get; set; }
        public string Incomplian { get; set; }
        public string Abovetank { get; set; }
        public string Tankmatdes { get; set; }
        public string Tankmodsde { get; set; }
        public string Pipematdes { get; set; }
        public string Pipemoddes { get; set; }
        public string Pipetypede { get; set; }
        public string Tankreldet { get; set; }
        public string Piperelde1 { get; set; }
        public string Piperelde2 { get; set; }
        public string PstFund { get; set; }
        public string Othertype { get; set; }
    }
}
