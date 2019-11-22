namespace QuartzJobMonitoring
{
    using System;

    public class JobDataStartDocument
    {
        public string JobId { get; set; }
        public string JobType { get; set; }
        public string JobName { get; set; }
        public DateTime StartDate { get; set; }
    }
}
