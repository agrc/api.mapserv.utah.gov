using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class PowerPlantsCo2
    {
        public int Xid { get; set; }
        public decimal? OrisCode { get; set; }
        public string State { get; set; }
        public string Partnership { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public decimal? Co2AnnualTons { get; set; }
        public decimal? Co2AnnualTonnes { get; set; }
        public string Fuelcode { get; set; }
        public string Variance { get; set; }
        public string GenFuel { get; set; }
        public string SpecFuel { get; set; }
        public string FuelAbbr { get; set; }
        public string DataAdjusted { get; set; }
        public Point Shape { get; set; }
    }
}
