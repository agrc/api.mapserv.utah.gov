namespace WebAPI.Dashboard.Areas.admin.Models
{
    public class UserStats
    {
        public string AccountId { get; set; }
        public long UsageCount { get; set; }
        public long LastUsed { get; set; }
    }
}