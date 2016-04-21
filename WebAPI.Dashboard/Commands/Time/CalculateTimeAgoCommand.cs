using System;
using WebAPI.Common.Abstractions;

namespace WebAPI.Dashboard.Commands.Time
{
    public class CalculateTimeAgoCommand : Command<string>
    {
        private const int Second = 1;
        private const int Minute = 60*Second;
        private const int Hour = 60*Minute;
        private const int Day = 24*Hour;
        private const int Month = 30*Day;
        private readonly double _delta;
        private readonly TimeSpan _timeSpan;

        public CalculateTimeAgoCommand(long dateLastUsedTicks)
        {
            DateLastUsed = new DateTime(dateLastUsedTicks);
            _timeSpan = new TimeSpan(DateTime.UtcNow.Ticks - DateLastUsed.Ticks);

            _delta = Math.Abs(_timeSpan.TotalSeconds);
        }

        public DateTime DateLastUsed { get; set; }

        protected override void Execute()
        {
            if (_delta < 0 || DateLastUsed == DateTime.MinValue)
            {
                Result = "never";
                return;
            }
            if (_delta < 1*Minute)
            {
                Result = _timeSpan.Seconds == 1 ? "one second ago" : _timeSpan.Seconds + " seconds ago";
                return;
            }
            if (_delta < 2*Minute)
            {
                Result = "a minute ago";
                return;
            }
            if (_delta < 45*Minute)
            {
                Result = _timeSpan.Minutes + " minutes ago";
                return;
            }
            if (_delta < 90*Minute)
            {
                Result = "an hour ago";
                return;
            }
            if (_delta < 24*Hour)
            {
                Result = _timeSpan.Hours + " hours ago";
                return;
            }
            if (_delta < 48*Hour)
            {
                Result = "yesterday";
                return;
            }
            if (_delta < 30*Day)
            {
                Result = _timeSpan.Days + " days ago";
                return;
            }
            if (_delta < 12*Month)
            {
                var months = Convert.ToInt32(Math.Floor((double) _timeSpan.Days/30));
                Result = months <= 1 ? "one month ago" : months + " months ago";
                return;
            }

            var years = Convert.ToInt32(Math.Floor((double) _timeSpan.Days/365));
            Result = years <= 1 ? "one year ago" : years + " years ago";
        }

        public override string ToString()
        {
            return string.Format("{0}, DateLastUsed: {1}", "CalculateTimeAgoCommand", DateLastUsed);
        }
    }
}