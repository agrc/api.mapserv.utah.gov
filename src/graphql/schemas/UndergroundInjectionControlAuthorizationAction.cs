using System;
using System.Collections.Generic;

namespace graphql.schemas
{
    public partial class UndergroundInjectionControlAuthorizationAction
    {
        public int Xid { get; set; }
        public string Guid { get; set; }
        public string AuthorizationFk { get; set; }
        public string Authorizationactiontype { get; set; }
        public DateTime? Authorizationactiondate { get; set; }
        public string Comments { get; set; }
        public DateTime? Createdon { get; set; }
        public DateTime? Modifiedon { get; set; }
        public string Editedby { get; set; }
    }
}
