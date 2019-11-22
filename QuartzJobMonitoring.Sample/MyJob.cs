namespace QuartzJobMonitoring.Sample
{
    using Quartz;
    using System.Threading.Tasks;

    [DisallowConcurrentExecution]
    public class MyJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            return Task.CompletedTask;
        }
    }
}
