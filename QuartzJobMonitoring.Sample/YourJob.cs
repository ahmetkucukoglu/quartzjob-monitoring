namespace QuartzJobMonitoring.Sample
{
    using Quartz;
    using System.Threading.Tasks;

    [DisallowConcurrentExecution]
    public class YourJob : IJob
    {
        private readonly IJobDataSource jobDataSource;

        public YourJob(IJobDataSource jobDataSource)
        {
            this.jobDataSource = jobDataSource;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await Task.Delay(5000);

            await jobDataSource.LogInformation(context, "TEST INFO");

            await Task.Delay(5000);

            await jobDataSource.LogWarning(context, "TEST WARN");
        }
    }
}
