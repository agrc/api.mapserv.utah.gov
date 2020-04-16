using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class DaqPermitCompApproval
    {
        public int Xid { get; set; }
        public decimal? DaqId { get; set; }
        public string Name { get; set; }
        public string MapLabel { get; set; }
        public string SiteAddress1 { get; set; }
        public string SiteAddress2 { get; set; }
        public string SiteAddress3 { get; set; }
        public string SiteCity { get; set; }
        public string SiteState { get; set; }
        public string SiteZip { get; set; }
        public string SiteMailingAddress1 { get; set; }
        public string SiteMailingAddress2 { get; set; }
        public string SiteMailingAddress3 { get; set; }
        public string SiteMailingCity { get; set; }
        public string SiteMailingState { get; set; }
        public string SiteMailingZip { get; set; }
        public string UtmZone { get; set; }
        public string SiteCounty { get; set; }
        public string OwnOperatorName { get; set; }
        public string OwnOperatorMailingAddress1 { get; set; }
        public string OwnOperatorMailingAddress2 { get; set; }
        public string OwnOperatorMailingAddress3 { get; set; }
        public string OwnOperatorMailingCity { get; set; }
        public string OwnOperatorMailingState { get; set; }
        public string OwnOperatorMailingZip { get; set; }
        public string PermitIssued { get; set; }
        public decimal? Easting { get; set; }
        public decimal? Northing { get; set; }
        public Point Shape { get; set; }
    }
}
