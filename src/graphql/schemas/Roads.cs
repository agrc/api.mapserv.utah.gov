using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class Roads
    {
        public int Xid { get; set; }
        public string Status { get; set; }
        public string Cartocode { get; set; }
        public string Fullname { get; set; }
        public decimal? FromaddrL { get; set; }
        public decimal? ToaddrL { get; set; }
        public decimal? FromaddrR { get; set; }
        public decimal? ToaddrR { get; set; }
        public string ParityL { get; set; }
        public string ParityR { get; set; }
        public string Predir { get; set; }
        public string Name { get; set; }
        public string Posttype { get; set; }
        public string Postdir { get; set; }
        public string AnName { get; set; }
        public string AnPostdir { get; set; }
        public string A1Predir { get; set; }
        public string A1Name { get; set; }
        public string A1Posttype { get; set; }
        public string A1Postdir { get; set; }
        public string A2Predir { get; set; }
        public string A2Name { get; set; }
        public string A2Posttype { get; set; }
        public string A2Postdir { get; set; }
        public string QuadrantL { get; set; }
        public string QuadrantR { get; set; }
        public string StateL { get; set; }
        public string StateR { get; set; }
        public string CountyL { get; set; }
        public string CountyR { get; set; }
        public string AddrsysL { get; set; }
        public string AddrsysR { get; set; }
        public string PostcommL { get; set; }
        public string PostcommR { get; set; }
        public string ZipcodeL { get; set; }
        public string ZipcodeR { get; set; }
        public string IncmuniL { get; set; }
        public string IncmuniR { get; set; }
        public string UninccomL { get; set; }
        public string UninccomR { get; set; }
        public string NbrhdcomL { get; set; }
        public string NbrhdcomR { get; set; }
        public string ErCadZones { get; set; }
        public string EsnL { get; set; }
        public string EsnR { get; set; }
        public string MsagcommL { get; set; }
        public string MsagcommR { get; set; }
        public string Oneway { get; set; }
        public string VertLevel { get; set; }
        public decimal? SpeedLmt { get; set; }
        public string Accesscode { get; set; }
        public string DotHwynam { get; set; }
        public string DotRtname { get; set; }
        public string DotRtpart { get; set; }
        public decimal? DotFMile { get; set; }
        public decimal? DotTMile { get; set; }
        public string DotFclass { get; set; }
        public string DotSrftyp { get; set; }
        public string DotClass { get; set; }
        public string DotOwnL { get; set; }
        public string DotOwnR { get; set; }
        public decimal? DotAadt { get; set; }
        public string DotAadtyr { get; set; }
        public decimal? DotThrulanes { get; set; }
        public string BikeL { get; set; }
        public string BikeR { get; set; }
        public string BikePlnL { get; set; }
        public string BikePlnR { get; set; }
        public string BikeRegpr { get; set; }
        public string BikeNotes { get; set; }
        public string UniqueId { get; set; }
        public string LocalUid { get; set; }
        public string UtahrdUid { get; set; }
        public string Source { get; set; }
        public DateTime? Updated { get; set; }
        public DateTime? Effective { get; set; }
        public DateTime? Expire { get; set; }
        public DateTime? Created { get; set; }
        public string Creator { get; set; }
        public string Editor { get; set; }
        public string Customtags { get; set; }
        public string TdmnetL { get; set; }
        public string TdmnetR { get; set; }
        public string PedL { get; set; }
        public string PedR { get; set; }
        public MultiLineString Shape { get; set; }
    }
}
