using System;
using System.Runtime.Serialization;
using System.Threading;

namespace WSUT.Upgrade.Soe.Models
{
    [DataContract(Namespace = "http://mapserv.utah.gov/WSUT/Geolocator")]
    public class GeocodeResult
    {
        private string _matchAddress;
        private double _longi, _lati;
        private readonly bool _doNotFormat;

        [DataMember(Order = 1)]
        public string MatchAddress
        {
            get { return _matchAddress; }
            set
            {
                if (_doNotFormat)
                {
                    _matchAddress = value.ToUpper();
                }
                else
                {
                    var cultureInfo = Thread.CurrentThread.CurrentCulture;
                    var titleCaser = cultureInfo.TextInfo;
                    _matchAddress = titleCaser.ToTitleCase(value.ToLower());
                }
            }
        }

        [DataMember(Order = 2)]
        public string Geocoder;
        [DataMember(Order = 3)]
        public double Score;

        [DataMember(Order = 4)]
        public double UTM_X;
        [DataMember(Order = 5)]
        public double UTM_Y;
        [DataMember(Order = 6)]
        public double LONG_X
        {
            get { return _longi; }
            set { _longi = Math.Round(value, 7); }
        }
        [DataMember(Order = 7)]
        public double LAT_Y
        {
            get { return _lati; }
            set { _lati = Math.Round(value, 7); }
        }

        public GeocodeResult()
        { }

        public GeocodeResult(string matchAddress, string geocoder, double score, bool doNotFormat=false)
        {
            _doNotFormat = doNotFormat;
            MatchAddress = matchAddress;
            Geocoder = geocoder;
            Score = score;
        }
    }
}
