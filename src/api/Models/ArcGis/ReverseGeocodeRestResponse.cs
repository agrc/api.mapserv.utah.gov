namespace AGRC.api.Models.ArcGis {
    public class ReverseGeocodeRestResponse : RestErrorable {
        public ReverseMatchAddress Address { get; set; }

        public Point Location { get; set; }

        public class ReverseMatchAddress {
            public string Street { get; set; }
            public string City { get; set; }
        }
    }
}
