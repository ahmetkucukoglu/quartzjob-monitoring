namespace QuartzJobMonitoring
{
    using System;

    public class JobDataExceptionDocument
    {
        public string JobId { get; set; }
        public string JobName { get; set; }
        public string Message { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
