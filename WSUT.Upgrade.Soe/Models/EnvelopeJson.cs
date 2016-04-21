namespace WSUT.Upgrade.Soe.Models
{
    public class EnvelopeJson
    {
        /// <summary>
        /// /// Serializable return object type containing your query results
        /// </summary>
        public EnvelopeJson() { }

        /// <summary>
        /// Serializable return object type containing your query results
        /// </summary>
        /// <param name="minx">Envelope min x value</param>
        /// <param name="miny">Envelope min y value</param>
        /// <param name="maxx">Envelope max x value</param>
        /// <param name="maxy">Envelope max y value</param>
        public EnvelopeJson(double minx, double miny, double maxx, double maxy)
        {
            MinX = minx;
            MinY = miny;
            MaxX = maxx;
            MaxY = maxy;
        }

        public double MinX { get; set; }

        public double MaxX { get; set; }

        public double MinY { get; set; }

        public double MaxY { get; set; }
    }
}