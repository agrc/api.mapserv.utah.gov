using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class EnterpriseZones
    {
        public int Xid { get; set; }
        public string County { get; set; }
        public string Zonename { get; set; }
        public string Expyr { get; set; }
        public string PocName { get; set; }
        public string PocPhone { get; set; }
        public string PocEmail { get; set; }
        public string CreatedUser { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastEditedUser { get; set; }
        public DateTime? LastEditedDate { get; set; }
        public string Valid { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
