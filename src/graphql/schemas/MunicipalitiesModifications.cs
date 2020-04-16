using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class MunicipalitiesModifications
    {
        public int Xid { get; set; }
        public string Countynbr { get; set; }
        public string Name { get; set; }
        public string Fips { get; set; }
        public DateTime? DateIn { get; set; }
        public DateTime? DateOut { get; set; }
        public string UpdateType { get; set; }
        public string DocName { get; set; }
        public string DocLoc { get; set; }
        public string CreatedUser { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastEditedUser { get; set; }
        public DateTime? LastEditedDate { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
