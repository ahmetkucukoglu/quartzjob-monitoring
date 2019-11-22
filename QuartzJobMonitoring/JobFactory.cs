namespace QuartzJobMonitoring
{
    using Quartz;
    using Quartz.Spi;
    using Microsoft.Extensions.DependencyInjection;
    using System;

    public class JobFactory : IJobFactory
    {
        private readonly IServiceProvider serviceProvider;

        public JobFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return serviceProvider.CreateScope().ServiceProvider.GetRequiredService(bundle.JobDetail.JobType) as IJob;
        }

        public void ReturnJob(IJob job)
        {
            GC.Collect();
        }
    }
}
