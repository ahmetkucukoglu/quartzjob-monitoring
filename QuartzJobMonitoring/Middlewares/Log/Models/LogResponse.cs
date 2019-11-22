namespace QuartzJobMonitoring
{
    using System;

    public class LogResponse
    {
        public string JobId { get; set; }
        public string JobName { get; set; }
        public string Level { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
