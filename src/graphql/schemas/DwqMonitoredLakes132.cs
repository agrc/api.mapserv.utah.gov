using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class DwqMonitoredLakes132
    {
        public int Xid { get; set; }
        public string Name { get; set; }
        public decimal? HucUnit { get; set; }
        public string LowCase { get; set; }
        public string MgmtUnit { get; set; }
        public string WbId { get; set; }
        public decimal? Acres { get; set; }
        public Point Shape { get; set; }
    }
}
