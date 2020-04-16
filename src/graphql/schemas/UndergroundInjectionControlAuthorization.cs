using System;
using System.Collections.Generic;

namespace graphql.schemas
{
    public partial class UndergroundInjectionControlAuthorization
    {
        public int Xid { get; set; }
        public string Guid { get; set; }
        public string FacilityFk { get; set; }
        public string Authorizationid { get; set; }
        public string Authorizationtype { get; set; }
        public string Ownersectortype { get; set; }
        public DateTime? Startdate { get; set; }
        public DateTime? Expirationdate { get; set; }
        public string Comments { get; set; }
        public DateTime? Createdon { get; set; }
        public DateTime? Modifiedon { get; set; }
        public string Editedby { get; set; }
    }
}
