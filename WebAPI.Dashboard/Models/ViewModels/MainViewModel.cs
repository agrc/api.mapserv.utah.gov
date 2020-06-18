using System;
using System.Collections.Generic;
using WebAPI.Common.Executors;
using WebAPI.Common.Models.Raven.Keys;
using WebAPI.Common.Models.Raven.Users;
using WebAPI.Dashboard.Areas.admin.Models;
using WebAPI.Dashboard.Commands.Time;
using WebAPI.Dashboard.Models.ViewModels.Keys;
using WebAPI.Dashboard.Models.ViewModels.Usage;

namespace WebAPI.Dashboard.Models.ViewModels
{
    public class MainViewModel : MainViewBase
    {
        public MainViewModel(Account account)
        {
            User = account;
        }

        public UserStats ApiKeyUsage { get; set; }

        public List<ApiKeyViewModel> AllUserKeys { get; set; }

        public Account User { get; set; }

        public AccountAccessBase Credentials { get; set; }

        public KeyQuota KeyQuota { get; set; }

        public MainViewModel WithApiUsage(UserStats item)
        {
            if (item == null)
            {
                item = new UserStats
                {
                    LastUsed = 0,
                    UsageCount = 0
                };
            }

            ApiKeyUsage = item;

            return this;
        }

        public MainViewModel WithAllUserKeys(List<UsageViewModel> item)
        {
            AllUserKeys = new List<ApiKeyViewModel>(item.Count);

            for (var i = 0; i < item.Count; i++)
            {
                var usage = item[i];
                AllUserKeys.Add(new ApiKeyViewModel
                {
                    ApiKey = usage.Key.Key,
                    LastUsed = CommandExecutor.ExecuteCommand(new CalculateTimeAgoCommand(usage.LastUsedTicks)),
                    UsageCount = Convert.ToInt32(usage.TotalUsageCount),
                    Type = usage.Key.Type.ToString(),
                    Active = usage.Key.ApiKeyStatus == ApiKey.KeyStatus.Active,
                    Development = usage.Key.AppStatus == ApiKey.ApplicationStatus.Development,
                    Pattern = usage.Key.Pattern
                });
            }

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