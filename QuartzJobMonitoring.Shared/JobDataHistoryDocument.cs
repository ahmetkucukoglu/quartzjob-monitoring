namespace QuartzJobMonitoring
{
    using System;

    public class JobDataHistoryDocument
    {
        public string JobId { get; set; }
        public string JobType { get; set; }
        public string JobName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public long ElapsedMilliseconds { get; set; }
    }
}
