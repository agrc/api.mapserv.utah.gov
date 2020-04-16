using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class BlmFieldOffices
    {
        public int Xid { get; set; }
        public decimal? Id { get; set; }
        public string Name { get; set; }
        public string OfficeTyp { get; set; }
        public Point Shape { get; set; }
    }
}
