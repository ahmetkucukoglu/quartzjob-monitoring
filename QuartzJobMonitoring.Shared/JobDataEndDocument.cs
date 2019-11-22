namespace QuartzJobMonitoring
{
    using System;

    public class JobDataEndDocument
    {
        public string JobId { get; set; }
        public string JobType { get; set; }
        public string JobName { get; set; }
        public DateTime EndDate { get; set; }
        public long ElapsedMilliseconds { get; set; }
    }
}
