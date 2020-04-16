using System;
using System.Collections.Generic;

namespace graphql.schemas
{
    public partial class UndergroundInjectionControlWellOperatingStatus
    {
        public int Xid { get; set; }
        public string Guid { get; set; }
        public string WellFk { get; set; }
        public string Operatingstatustype { get; set; }
        public DateTime? Operatingstatusdate { get; set; }
        public string Comments { get; set; }
        public DateTime? Createdon { get; set; }
        public DateTime? Modifiedon { get; set; }
        public string Editedby { get; set; }
    }
}
