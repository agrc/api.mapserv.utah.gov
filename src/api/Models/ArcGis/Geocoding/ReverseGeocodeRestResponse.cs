namespace AGRC.api.Models.ArcGis {
    public class ReverseGeocodeRestResponse : RestErrorable {
        public ReverseMatchAddress Address { get; set; }

        public Point Location { get; set; }

        public class ReverseMatchAddress {
            public string Address { get; set; }
            public string City { get; set; }
            public string Addr_type { get; set; }
        }
    }
}
