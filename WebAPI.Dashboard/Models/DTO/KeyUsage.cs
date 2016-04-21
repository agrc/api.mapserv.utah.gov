using WebAPI.Common.Models.Raven.Keys;

namespace WebAPI.Dashboard.Models.DTO
{
    public class KeyUsage
    {
        public int Usage { get; set; }
        public ApiKey ApiKey { get; set; }
        public int Bucket { get; set; }
    }
}