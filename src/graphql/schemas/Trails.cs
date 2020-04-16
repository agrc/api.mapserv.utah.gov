using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class Trails
    {
        public int Xid { get; set; }
        public string Primaryname { get; set; }
        public string Trailid { get; set; }
        public string Systemname { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public string Designateduses { get; set; }
        public string Surfacetype { get; set; }
        public string Primarymaint { get; set; }
        public string Trailclass { get; set; }
        public string Transnetwork { get; set; }
        public string Adaaccessible { get; set; }
        public string Bikedifficulty { get; set; }
        public string Hikedifficulty { get; set; }
        public string Otherrestrictions { get; set; }
        public string Roadconcurrent { get; set; }
        public string Infourl { get; set; }
        public string Comments { get; set; }
        public string Datasource { get; set; }
        public DateTime? Lastupdate { get; set; }
        public string Cartocode { get; set; }
        public string UsngSeg { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastEditedDate { get; set; }
        public string Recreationarea { get; set; }
        public string Motorizedallowed { get; set; }
        public string Horseallowed { get; set; }
        public MultiLineString Shape { get; set; }
    }
}
