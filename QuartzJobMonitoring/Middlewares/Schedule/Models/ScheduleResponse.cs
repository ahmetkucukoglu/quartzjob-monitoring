namespace QuartzJobMonitoring
{
    using System.Collections.Generic;

    public class ScheduleResponse
    {
        public ScheduleResponse()
        {
            Cron = new List<ScheduleCronResponse>();
        }

        public string JobName { get; set; }
        public List<ScheduleCronResponse> Cron { get; set; }
    }
}
