namespace QuartzJobMonitoring
{
    using Quartz;
    using Quartz.Spi;
    using Microsoft.Extensions.Hosting;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public class JobHostedService : IHostedService
    {
        private readonly ISchedulerFactory schedulerFactory;
        private readonly IJobFactory jobFactory;
        private readonly IJobListener jobListener;
        private readonly IEnumerable<JobSchedule> jobSchedules;

        public JobHostedService(
            ISchedulerFactory schedulerFactory,
            IJobFactory jobFactory,
            IJobListener jobListener,
            IEnumerable<JobSchedule> jobSchedules)
        {
            this.schedulerFactory = schedulerFactory;
            this.jobFactory = jobFactory;
            this.jobListener = jobListener;
            this.jobSchedules = jobSchedules;
        }

        public IScheduler Scheduler { get; set; }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Scheduler = await schedulerFactory.GetScheduler(cancellationToken);
            Scheduler.JobFactory = jobFactory;
            Scheduler.ListenerManager.AddJobListener(jobListener);

            for (int i = 0; i < jobSchedules.Count(); i++)
            {
                var jobSchedule = jobSchedules.ElementAt(i);
                var job = CreateJob(jobSchedule);

                await Scheduler.AddJob(job, false, cancellationToken);

                foreach (var trigger in CreateTriggers(jobSchedule, job))
                {
                    await Scheduler.ScheduleJob(trigger, cancellationToken);
                }
            }

            await Scheduler.Start(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Scheduler?.Shutdown(cancellationToken);
        }

        private static IJobDetail CreateJob(JobSchedule schedule)
        {
            var jobType = schedule.JobType;

            var jobBuilder = JobBuilder
                .Create(jobType)
                .StoreDurably()
                .WithIdentity($"{schedule.JobName}")
                .WithDescription(jobType.Name);

            if (schedule.Retry.HasValue)
                jobBuilder = jobBuilder.UsingJobData("Retry", schedule.Retry.Value);

            if(schedule.Data != null)
                jobBuilder.SetJobData(new JobDataMap((IDictionary<string, object>)schedule.Data));
            
            return jobBuilder.Build();
        }

        private IEnumerable<ITrigger> CreateTriggers(JobSchedule schedule, IJobDetail jobDetail)
        {
            foreach (var cronExpression in schedule.CronExpressions)
            {
                yield return TriggerBuilder
                    .Create()
                    .WithCronSchedule(cronExpression)
                    .WithDescription(cronExpression)
                    .ForJob(jobDetail)
                    .Build();
            }
        }
    }
}
