using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class Cemeteries
    {
        public int Xid { get; set; }
        public string Cemetery { get; set; }
        public string City { get; set; }
        public string County { get; set; }
        public decimal? NumberOfBurials { get; set; }
        public string Address { get; set; }
        public string LocationDirections { get; set; }
        public string Contact { get; set; }
        public string Owner { get; set; }
        public string Phone { get; set; }
        public string YearEstablishedFirstBurial { get; set; }
        public string Status { get; set; }
        public Point Shape { get; set; }
    }
}
