using System.IO;
using System.Web;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.Email;
using Serilog.Sinks.GoogleCloudLogging;

namespace WebAPI.Common.Logging
{
    public static class LoggingConfig
    {
        public static void Register(string name)
        {
            var config = new GoogleCloudLoggingSinkOptions
            {
                UseJsonOutput = true,
                LogName = "api.mapserv.utah.gov",
                UseSourceContextAsLogName = false,
                ResourceType = "global",
                ServiceName = "api.mapserv.utah.gov",
                ServiceVersion = "1.12.5"
            };

#if DEBUG
            var projectId = "ut-dts-agrc-web-api-dv";
            var fileName = "ut-dts-agrc-web-api-dv-log-writer.json";
            var serviceAccount = File.ReadAllText(Path.Combine(HttpRuntime.AppDomainAppPath, fileName));
            config.GoogleCredentialJson = serviceAccount;
#else
            var projectId = "ut-dts-agrc-web-api-prod";
#endif
            config.ProjectId = projectId;

            var email = new EmailConnectionInfo
            {
                EmailSubject = "Geocoding Log Email",
                FromEmail = "noreply@utah.gov",
                ToEmail = "SGourley@utah.gov"
            };

            var levelSwitch = new LoggingLevelSwitch();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.ControlledBy(levelSwitch)
                .WriteTo.Email(email, restrictedToMinimumLevel: LogEventLevel.Error)
                .WriteTo.GoogleCloudLogging(config)
                .CreateLogger();

#if DEBUG
            levelSwitch.MinimumLevel = LogEventLevel.Verbose;
#else
            levelSwitch.MinimumLevel = LogEventLevel.Information;
#endif

            Log.Debug("Logging initialized");
        }
    }
}
