﻿using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class PlssPointAgrc
    {
        public int Xid { get; set; }
        public string Pointlab { get; set; }
        public string Pointid { get; set; }
        public string Plssid { get; set; }
        public decimal? Xcoord { get; set; }
        public decimal? Ycoord { get; set; }
        public decimal? Zcoord { get; set; }
        public decimal? Elev { get; set; }
        public string Hdatum { get; set; }
        public string Vdatum { get; set; }
        public string Steward1 { get; set; }
        public string Steward2 { get; set; }
        public string Local1 { get; set; }
        public string Local2 { get; set; }
        public string Local3 { get; set; }
        public string Local4 { get; set; }
        public string Rely { get; set; }
        public string Coordproc { get; set; }
        public string Coordsys { get; set; }
        public string Coordmeth { get; set; }
        public DateTime? Reviseddate { get; set; }
        public decimal? Errorx { get; set; }
        public decimal? Errory { get; set; }
        public decimal? Errorz { get; set; }
        public string CoordSource { get; set; }
        public string TiesheetName { get; set; }
        public string DisplayGrp { get; set; }
        public string LongNad83 { get; set; }
        public string LatNad83 { get; set; }
        public string PointCategory { get; set; }
        public string Ismonument { get; set; }
        public string Iscontrol { get; set; }
        public string County { get; set; }
        public Point Shape { get; set; }
    }
}
