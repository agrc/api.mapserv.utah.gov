using System;
using System.Collections.Generic;

namespace graphql.schemas
{
    public partial class EnvironmentUstRiskAssessment
    {
        public int Xid { get; set; }
        public decimal? Facid { get; set; }
        public string Altfacilityid { get; set; }
        public string FacilityName { get; set; }
        public string FacilityAddress { get; set; }
        public string FacilityCity { get; set; }
        public decimal? Tankriskavg { get; set; }
        public decimal? Severityratio { get; set; }
        public decimal? Facavg { get; set; }
        public string Matofconst { get; set; }
        public string Matofconstcount { get; set; }
        public decimal? Age { get; set; }
        public decimal? Agecount { get; set; }
        public decimal? Spillinstalled { get; set; }
        public decimal? Spillinstalledcount { get; set; }
        public decimal? Contdisp { get; set; }
        public decimal? Contdispcount { get; set; }
        public decimal? Contsump { get; set; }
        public decimal? Contsumpcount { get; set; }
        public decimal? Overfillinstalled { get; set; }
        public decimal? Overfillinstalledcount { get; set; }
        public string Pipemoddesc { get; set; }
        public decimal? Pipemoddesccount { get; set; }
        public string Pipematdesc { get; set; }
        public decimal? Pipematdesccount { get; set; }
        public string Tankmodsdesc { get; set; }
        public decimal? Tankmodsdesccount { get; set; }
        public string Wrpod { get; set; }
        public decimal? Wrpodcount { get; set; }
        public decimal? Diststream { get; set; }
        public decimal? Diststreamcount { get; set; }
        public decimal? Distlake { get; set; }
        public decimal? Distlakecount { get; set; }
        public decimal? Popdensity { get; set; }
        public decimal? Popdensitycount { get; set; }
        public decimal? Depthtowat { get; set; }
        public decimal? Depthtowatcount { get; set; }
        public string Soils { get; set; }
        public decimal? Soilscount { get; set; }
        public string Assessunits { get; set; }
        public decimal? Assessunitscount { get; set; }
        public string Wetlands { get; set; }
        public decimal? Wetlandscount { get; set; }
        public string Swpz { get; set; }
        public decimal? Swpzcount { get; set; }
        public string Aqz { get; set; }
        public decimal? Aqzcount { get; set; }
        public decimal? Clust { get; set; }
        public decimal? Clustcount { get; set; }
        public decimal? Lust { get; set; }
        public decimal? Lustcount { get; set; }
        public decimal? Sevage { get; set; }
        public decimal? Sevagecount { get; set; }
        public string Sevmat { get; set; }
        public decimal? Sevmatcount { get; set; }
        public decimal? Complpts { get; set; }
        public decimal? Complptscount { get; set; }
        public DateTime? Assessdate { get; set; }
    }
}
