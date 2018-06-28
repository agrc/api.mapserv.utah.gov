using System;
using developer.mapserv.utah.gov.Areas.Secure.Formatters;

namespace developer.mapserv.utah.gov.Areas.Secure.Models.ViewModels
{
    public class IndexViewModel : ValueFormatter
    {
        public IndexViewModel(int? totalRequests, int? keysUsed)
        {
            RequestsToDate = CommasNoDecimals(totalRequests);
            KeysUsed = DefaultIfNull(keysUsed);
        }

        public string RequestsToDate { get; set; }
        public string KeysUsed { get; set; }
    }
}
