using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class WeatherStations
    {
        public int Xid { get; set; }
        public string StationNa { get; set; }
        public string StnId { get; set; }
        public string Secondary { get; set; }
        public string StnAuthor { get; set; }
        public string Network { get; set; }
        public string LocationO { get; set; }
        public string TypeOfOb { get; set; }
        public string ActiveIna { get; set; }
        public string PeriodOf { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string County { get; set; }
        public decimal? ClimateDi { get; set; }
        public string WaterBasi { get; set; }
        public string Huc { get; set; }
        public string Temp { get; set; }
        public string Precipitat { get; set; }
        public string SnowFall { get; set; }
        public string SnwGndDp { get; set; }
        public string DewPoint { get; set; }
        public string RelHum { get; set; }
        public string WindSpeed { get; set; }
        public string WindGust { get; set; }
        public string SolarRad { get; set; }
        public string SoilMoist { get; set; }
        public string SoilTemp { get; set; }
        public string FuelTemp { get; set; }
        public string FuelMoist { get; set; }
        public string Pressure { get; set; }
        public string SlPressur { get; set; }
        public string Altimeter { get; set; }
        public string WeatherCo { get; set; }
        public string Visibility { get; set; }
        public string Uvb { get; set; }
        public string Elevation { get; set; }
        public Point Shape { get; set; }
    }
}
