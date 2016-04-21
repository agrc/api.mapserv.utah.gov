using System;

namespace WebAPI.Common.Models.Changelog
{
    public class LogEntry
    {
        public LogEntry(string logMessage, DateTime time)
        {
            LogMessage = logMessage;
            Time = time;
        }

        public string LogMessage { get; set; }
        public DateTime Time { get; set; }
    }
}