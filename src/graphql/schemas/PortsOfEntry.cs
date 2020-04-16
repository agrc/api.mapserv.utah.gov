using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class PortsOfEntry
    {
        public int Xid { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public decimal? Zip { get; set; }
        public string Supervisor { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public decimal? Lat { get; set; }
        public decimal? Lon { get; set; }
        public Point Shape { get; set; }
    }
}
