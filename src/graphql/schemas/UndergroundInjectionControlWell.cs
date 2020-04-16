using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class UndergroundInjectionControlWell
    {
        public int Xid { get; set; }
        public string Guid { get; set; }
        public string FacilityFk { get; set; }
        public string AuthorizationFk { get; set; }
        public string Wellid { get; set; }
        public string Wellname { get; set; }
        public decimal? Wellclass { get; set; }
        public decimal? Wellsubclass { get; set; }
        public decimal? Surfaceelevation { get; set; }
        public string Highpriority { get; set; }
        public string Convertedogwell { get; set; }
        public string Remediationprojectid { get; set; }
        public decimal? Remediationprojecttype { get; set; }
        public string Wellswpz { get; set; }
        public string Injectionaquiferexempt { get; set; }
        public string Locationmethod { get; set; }
        public string Locationaccuracy { get; set; }
        public string Comments { get; set; }
        public DateTime? Createdon { get; set; }
        public DateTime? Modifiedon { get; set; }
        public string Editedby { get; set; }
        public string AorFk { get; set; }
        public Point Shape { get; set; }
    }
}
