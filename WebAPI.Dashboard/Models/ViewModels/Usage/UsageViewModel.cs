using System;
using WebAPI.Common.Models.Raven.Keys;

namespace WebAPI.Dashboard.Models.ViewModels.Usage
{
    public class UsageViewModel
    {
        public UsageViewModel(ApiKey key)
        {
            Key = key;
        }

        public ApiKey Key { get; set; }
        public long LastUsedTicks { get; set; }
        public long TotalUsageCount { get; set; }
        public long UsageToday { get; set; }
        public long UsageForMonth { get; set; }
        public double TotalGeocodeUsage { get; set; }
        public double TotalSearchUsage { get; set; }
        public double TotalInfoUsage { get; set; }

        public string PercentGeocoding
        {
            get { return ToPercentage(TotalGeocodeUsage, TotalNewUsageCount); }
        }
    
        public string PercentSearching
        {
            get { return ToPercentage(TotalSearchUsage, TotalNewUsageCount); }
        }

        public string PercentInfo
        {
            get { return ToPercentage(TotalInfoUsage, TotalNewUsageCount); }
        }
    
        public double TotalNewUsageCount
        {
            get { return TotalGeocodeUsage + TotalSearchUsage + TotalInfoUsage; }
        }

        private static string ToPercentage(double amount, double total)
        {
            if (total < 1 || amount < 1)
            {
                return "0";
            }

            return ((amount/total)).ToString("0.00%");
        }

        public bool ShowAdvancedStats
        {
            get { return TotalNewUsageCount > 0; }
        }

        public long UsageNow { get; set; }
    }
}