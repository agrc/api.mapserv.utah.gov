using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class UsgsDemExtents
    {
        public int Xid { get; set; }
        public string Resolution { get; set; }
        public string Description { get; set; }
        public string YearCollected { get; set; }
        public string FileFormat { get; set; }
        public string FileExtension { get; set; }
        public string AverageFileSize { get; set; }
        public string TotalSize { get; set; }
        public string HorizontalAccuracy { get; set; }
        public string VerticalAccuracy { get; set; }
        public string TotalSquareMiles { get; set; }
        public string Contact { get; set; }
        public string InHouse { get; set; }
        public string FtpPath { get; set; }
        public string Product { get; set; }
        public string Category { get; set; }
        public DateTime? EstimatedDate { get; set; }
        public string HtmlPage { get; set; }
        public string RestEndpoint { get; set; }
        public string LyrFile { get; set; }
        public string FlightdateLocation { get; set; }
        public string TileIndex { get; set; }
        public string Interval { get; set; }
        public DateTime? UploadDate { get; set; }
        public string Metadata { get; set; }
        public string Report { get; set; }
        public string Show { get; set; }
        public string Servicename { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
