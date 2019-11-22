namespace QuartzJobMonitoring
{
    public class JobResponse
    {
        public string Name { get; set; }
        public string LastRun { get; set; }
        public StatisticResponse Statistic { get; set; }
    }
}
