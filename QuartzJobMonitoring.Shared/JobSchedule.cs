namespace QuartzJobMonitoring
{
    using System;
    using System.Collections.Generic;

    public class JobSchedule
    {
        public JobSchedule(Type jobType, string jobName, string[] cronExpressions, Dictionary<string, object> data, int retry)
        {
            JobType = jobType;
            CronExpressions = cronExpressions;
            JobName = jobName;
            Retry = retry;
            Data = data;
        }

        public Type JobType { get; }
        public string JobName { get; set; }
        public string[] CronExpressions { get; }
        public Dictionary<string, object> Data { get; }
        public int? Retry { get; set; }
    }
}
