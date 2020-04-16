using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class AirportLocations
    {
        public int Xid { get; set; }
        public string SiteNo { get; set; }
        public string LanFaTy { get; set; }
        public string Locid { get; set; }
        public string EffDate { get; set; }
        public string FaaRegion { get; set; }
        public string FaaDistri { get; set; }
        public string StPostal { get; set; }
        public string Stfips { get; set; }
        public string FaaSt { get; set; }
        public string StateName { get; set; }
        public string CountyNam { get; set; }
        public string CountySt { get; set; }
        public string CityName { get; set; }
        public string Fullname { get; set; }
        public string OwnerType { get; set; }
        public string FacUse { get; set; }
        public string FacCystzp { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string Elev { get; set; }
        public string AeroChart { get; set; }
        public string CbdDist { get; set; }
        public string CbdDir { get; set; }
        public string ActDate { get; set; }
        public string CertType { get; set; }
        public string FedAgree { get; set; }
        public string Internatio { get; set; }
        public string CustLndg { get; set; }
        public string JointUse { get; set; }
        public string MilLndgR { get; set; }
        public string CntlTwr { get; set; }
        public string SEngGa { get; set; }
        public string MEngGa { get; set; }
        public string JetEnGa { get; set; }
        public string Helicopter { get; set; }
        public string OperGlide { get; set; }
        public string OperMil { get; set; }
        public string Ultralight { get; set; }
        public string CommServ { get; set; }
        public string AirTaxi { get; set; }
        public string LocalOps { get; set; }
        public string ItinOps { get; set; }
        public string MilOps { get; set; }
        public decimal? Arrivals { get; set; }
        public decimal? Departures { get; set; }
        public decimal? Enplanemen { get; set; }
        public decimal? Passengers { get; set; }
        public Point Shape { get; set; }
    }
}
