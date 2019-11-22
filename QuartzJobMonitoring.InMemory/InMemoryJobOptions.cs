namespace QuartzJobMonitoring
{
    using Microsoft.Extensions.DependencyInjection;

    public static class InMemoryJobOptions
    {
        public static void UseInMemory(this QuartzJobMonitoringOptions quartzJobOptions)
        {
            quartzJobOptions.services.AddSingleton<IJobDataSource, InMemoryJobDataSource>();
        }
    }
}
