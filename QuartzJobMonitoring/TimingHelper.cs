namespace QuartzJobMonitoring
{
    using System;

    public static class TimingHelper
    {
        const int SECOND = 1;
        const int MINUTE = 60 * SECOND;
        const int HOUR = 60 * MINUTE;
        const int DAY = 24 * HOUR;
        const int MONTH = 30 * DAY;

        public static string GetTiming(DateTime dateTime)
        {
            var timeSpan = new TimeSpan(DateTime.Now.Ticks - dateTime.Ticks);
            var delta = Math.Abs(timeSpan.TotalSeconds);

            if (delta < 1 * MINUTE)
                return timeSpan.Seconds == 1 ? "a second ago" : timeSpan.Seconds + " second ago";

            if (delta < 2 * MINUTE)
                return "a minute ago";

            if (delta < 45 * MINUTE)
                return timeSpan.Minutes + " minute ago";

            if (delta < 90 * MINUTE)
                return "a hour ago";

            if (delta < 24 * HOUR)
                return timeSpan.Hours + " hour ago";

            if (delta < 48 * HOUR)
                return "tomorrow";

            if (delta < 30 * DAY)
                return timeSpan.Days + " day ago";

            if (delta < 12 * MONTH)
            {
                int months = Convert.ToInt32(Math.Floor((double)timeSpan.Days / 30));
                return months <= 1 ? "a month ago" : months + " month ago";
            }
            else
            {
                int years = Convert.ToInt32(Math.Floor((double)timeSpan.Days / 365));
                return years <= 1 ? "a year ago" : years + " year ago";
            }
        }

        public static string GetTiming(long ellapsedMilliseconds)
        {
            var hours = Math.Floor((decimal)(ellapsedMilliseconds / (60 * 60 * 1000)));

            var divisorForMinutes = ellapsedMilliseconds % (60 * 60 * 1000);
            var minutes = Math.Floor((decimal)(divisorForMinutes / 60 / 1000));

            var divisorForSeconds = divisorForMinutes % (60 * 1000);
            var seconds = Math.Ceiling((decimal)(divisorForSeconds / 1000));

            var time = string.Empty;

            if (hours > 0)
                time += hours + " hour ";

            if (minutes > 0)
                time += minutes + " minute ";

            if (seconds >= 0)
                time += seconds + " second ";

            return time;
        }
    }
}
