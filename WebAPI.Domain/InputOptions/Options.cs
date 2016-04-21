namespace WebAPI.Domain.InputOptions
{
    /// <summary>
    /// Base options for geocoding
    /// </summary>
    public class Options
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Options"/> class.
        /// </summary>
        public Options()
        {
            JsonFormat = JsonFormat.None;   
        }
 
         /// <summary>
        ///     Gets or sets the json format for the geometry response.
        /// </summary>
        /// <value>
        ///     The json format.
        /// </value>
        public JsonFormat JsonFormat { get; set; }
    }

}