using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class Schools
    {
        public int Xid { get; set; }
        public string District { get; set; }
        public string School { get; set; }
        public string Leanum { get; set; }
        public string Schoolnum { get; set; }
        public string Schooltype { get; set; }
        public string Edtype { get; set; }
        public string Gradelow { get; set; }
        public string Gradehigh { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
        public string Phone { get; set; }
        public string Web { get; set; }
        public Point Shape { get; set; }
    }
}
