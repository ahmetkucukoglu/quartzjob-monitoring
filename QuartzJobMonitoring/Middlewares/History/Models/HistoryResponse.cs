namespace QuartzJobMonitoring
{
    using System;

    public class HistoryResponse
    {
        public string JobId { get; set; }
        public string JobName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public long ElapsedMilliseconds { get; set; }
        public string LastRun { get; set; }
        public string ElapsedTime { get; set; }
    }
}
