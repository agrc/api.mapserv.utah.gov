namespace WebAPI.Domain.InputOptions
{
    public class SearchOptions
    {
        private string _predicate;
        private double _buffer;

        public SearchOptions()
        {
            Geometry = "";
            WkId = 26912;
            Predicate = "";
        }

        /// <summary>
        ///     Gets or sets the predicate. The where clause of the statement
        /// </summary>
        /// <value>
        ///     The predicate.
        /// </value>
        public string Predicate
        {
            get { return _predicate; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _predicate = value;
                }
            }
        }

        /// <summary>
        ///     Gets or sets the point. The coordinate pair representing a point. 
        /// points:[x,y]
        /// </summary>
        /// <value>
        ///     The point.
        /// </value>
        public string Geometry { get; set; }

        /// <summary>
        ///     Gets or sets the spatial reference well known id.
        /// </summary>
        /// <value>
        ///     The wkid.
        /// </value>
        public int WkId { get; set; }

        /// <summary>
        /// Gets or sets the buffer.
        /// </summary>
        /// <value>
        /// The value in meters to buffer the input geometry.
        /// </value>
        public double Buffer
        {
            get { return _buffer; }
            set
            {
                if (value > 2000)
                {
                    _buffer = 2000;
                    
                    return;
                }

                _buffer = value;
            }
        }

        /// <summary>
        /// Gets or sets the attribute style.
        /// </summary>
        /// <value>
        /// The attribute style. Identical, Camel, Upper, Lower
        /// </value>
        public AttributeStyle AttributeStyle { get; set; }

        public override string ToString()
        {
            return string.Format("[SearchOptions] Predicate: {0}, Geometry: {1}, WkId: {2}, Buffer: {3}", Predicate, Geometry, WkId, Buffer);
        }
    }
}