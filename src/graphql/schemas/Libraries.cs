using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class Libraries
    {
        public int Xid { get; set; }
        public string Status { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string PoBox { get; set; }
        public string City { get; set; }
        public string County { get; set; }
        public string ZipCode { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Director { get; set; }
        public string AsstDirec { get; set; }
        public string DirectorEmail { get; set; }
        public string Website { get; set; }
        public string HoursMond { get; set; }
        public string HoursTues { get; set; }
        public string HoursWedn { get; set; }
        public string HoursThur { get; set; }
        public string HoursFrid { get; set; }
        public string HoursSatu { get; set; }
        public string HoursSund { get; set; }
        public string LibraryId { get; set; }
        public Point Shape { get; set; }
    }
}
