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
            var config = new GoogleCloudLoggingSinkOptions {
                ProjectId = "ut-dts-agrc-web-api-prod",
                UseJsonOutput = true,
                LogName = "api.mapserv.utah.gov",
                UseSourceContextAsLogName = false,
                ResourceType = "global",
                ServiceName = "api.mapserv.utah.gov",
                ServiceVersion = "1.12.0"
            };

            var email = new EmailConnectionInfo
            {
                EmailSubject = "Geocoding Log Email",
                FromEmail = "noreply@utah.gov",
                ToEmail = "SGourley@utah.gov"
            };

            var dir = Path.Combine(HttpRuntime.AppDomainAppPath, $@"..\logs\geocoding\{name}.log-{{Date}}.txt");
            var levelSwitch = new LoggingLevelSwitch();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.ControlledBy(levelSwitch)
                // .WriteTo.RollingFile(dir)
                .WriteTo.Email(email, restrictedToMinimumLevel: LogEventLevel.Error)
                .WriteTo.GoogleCloudLogging(config)
                .CreateLogger();

#if DEBUG
            levelSwitch.MinimumLevel = LogEventLevel.Verbose;
#else
            levelSwitch.MinimumLevel = LogEventLevel.Warning;
#endif

            Log.Debug("Logging initialized");
        }
    }
}