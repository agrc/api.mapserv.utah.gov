using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class UsfsRoadlessInventory
    {
        public int Xid { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Forest { get; set; }
        public string Ownership { get; set; }
        public string Comment { get; set; }
        public decimal? Region { get; set; }
        public string RevCat { get; set; }
        public string RevCatCom { get; set; }
        public string Feature1 { get; set; }
        public string Name1 { get; set; }
        public string Comment1 { get; set; }
        public string Authority1 { get; set; }
        public string Feature2 { get; set; }
        public string Name2 { get; set; }
        public string Comment2 { get; set; }
        public string Authority2 { get; set; }
        public string Feature3 { get; set; }
        public string Name3 { get; set; }
        public string Comment3 { get; set; }
        public string Authority3 { get; set; }
        public string State { get; set; }
        public string FinalCat { get; set; }
        public decimal? Acres { get; set; }
        public string IraName2 { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
