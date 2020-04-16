using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class WetlandsMappingProjects
    {
        public int Xid { get; set; }
        public string ProjectName { get; set; }
        public string Status { get; set; }
        public string Suppmapinfo { get; set; }
        public string DataSource { get; set; }
        public decimal? ImageYr { get; set; }
        public string ImageDate { get; set; }
        public decimal? ImageScale { get; set; }
        public string AllScales { get; set; }
        public string Emulsion { get; set; }
        public string Comments { get; set; }
        public string SourceType { get; set; }
        public string DataCat { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
