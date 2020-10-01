namespace WebAPI.Domain.ArcServerResponse.Soe
{
    public class GeocodeMilepostResponse
    {
        public string MatchAddress;
        public string Geocoder;
        public double Score;
        public double UTM_X;
        public double UTM_Y;
        public double LONG_X;
        public double LAT_Y;
    }
}