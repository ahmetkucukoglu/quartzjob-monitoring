namespace QuartzJobMonitoring.Test
{
    using Quartz;
    using System.Threading.Tasks;

    [DisallowConcurrentExecution]
    public class TestJob : IJob
    {
        private readonly IJobDataSource jobDataSource;

        public TestJob(IJobDataSource jobDataSource)
        {
            this.jobDataSource = jobDataSource;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await jobDataSource.LogInformation(context, "Test Information");
            await jobDataSource.LogWarning(context, "Test Warning");
        }
    }
}
