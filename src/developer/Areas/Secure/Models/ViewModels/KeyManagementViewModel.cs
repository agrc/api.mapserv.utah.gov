using System;
using System.Collections.Generic;
using developer.mapserv.utah.gov.Areas.Secure.Formatters;
using developer.mapserv.utah.gov.Areas.Secure.Models.Database;

namespace developer.mapserv.utah.gov.Areas.Secure.Models.ViewModels
{
    public class KeyManagementViewModel : ValueFormatter
    {
        public KeyManagementViewModel(KeyQuotaDTO quota, IEnumerable<ApiKeyDTO> keys)
        {
            KeysUsed = DefaultIfNull(quota.KeysUsed);
            KeysAllowed = quota.KeysAllowed.ToString();
            Keys = keys;
        }

        public string KeysAllowed { get; set; }
        public string KeysUsed { get; set; }
        public IEnumerable<ApiKeyDTO> Keys { get; }
    }
}
