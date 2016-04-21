using System.Collections.Generic;
using AutoMapper;
using WebAPI.Common.Indexes;
using WebAPI.Common.Models.Raven.Users;
using WebAPI.Dashboard.Models.ViewModels.Keys;

namespace WebAPI.Dashboard.Models.ViewModels
{
    public class MainViewModel : MainViewBase
    {
        public MainViewModel(Account account)
        {
            User = account;
        }

        public StatsPerUser.Stats ApiKeyUsage { get; set; }

        public List<ApiKeyViewModel> AllUserKeys { get; set; }

        public Account User { get; set; }

        public AccountAccessBase Credentials { get; set; }

        public KeyQuota KeyQuota { get; set; }

        public MainViewModel WithApiUsage(StatsPerUser.Stats item)
        {
            if (item == null)
            {
                item = new StatsPerUser.Stats
                {
                    LastUsed = 0,
                    UsageCount = 0
                };
            }

            ApiKeyUsage = item;
            return this;
        }

        public MainViewModel WithAllUserKeys(List<StatsPerApiKey.Stats> item)
        {
            AllUserKeys = new List<ApiKeyViewModel>();

            AllUserKeys = Mapper.Map<List<StatsPerApiKey.Stats>, List<ApiKeyViewModel>>(item);

            return this;
        }

        public MainViewModel WithUser(Account item)
        {
            User = item;
            return this;
        }

        public MainViewModel WithAccountAccessBase(AccountAccessBase item)
        {
            Credentials = item;
            return this;
        }

        public MainViewModel WithKeyQuota(KeyQuota item)
        {
            KeyQuota = item;
            return this;
        }
    }
}