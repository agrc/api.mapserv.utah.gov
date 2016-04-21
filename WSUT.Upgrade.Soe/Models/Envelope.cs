using System.Xml.Serialization;

namespace WSUT.Upgrade.Soe.Models
{
    /// <summary>
    /// /// Serializable return object type containing your query results
    /// </summary>
    [XmlInclude(typeof(Envelope))]
    public class Envelope
    {
        /// <summary>
        /// /// Serializable return object type containing your query results
        /// </summary>
        public Envelope() { }

        /// <summary>
        /// Serializable return object type containing your query results
        /// </summary>
        /// <param name="minx">Envelope min x value</param>
        /// <param name="miny">Envelope min y value</param>
        /// <param name="maxx">Envelope max x value</param>
        /// <param name="maxy">Envelope max y value</param>
        /// <param name="message">Error message.  If null no errors</param>
        public Envelope(double minx, double miny, double maxx, double maxy, string message)
        {
            MinX = minx;
            MinY = miny;
            MaxX = maxx;
            MaxY = maxy;
            Message = message;
        }

        public double MinX { get; set; }

        public double MaxX { get; set; }

        public double MinY { get; set; }

        public double MaxY { get; set; }

        public string Message { get; set; }
    }
}