namespace QuartzJobMonitoring
{
    using Quartz;
    using Quartz.Impl.Triggers;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    public class JobListener : IJobListener
    {
        private readonly Dictionary<string, Stopwatch> _stopwatches = new Dictionary<string, Stopwatch>();
        private readonly IJobDataSource jobDataSource;

        public string Name => "JobListener";

        public JobListener(IJobDataSource jobDataSource)
        {
            this.jobDataSource = jobDataSource;
        }

        public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public async Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            var startDocument = new JobDataStartDocument
            {
                JobId = context.FireInstanceId,
                JobType = context.JobDetail.JobType.Name,
                JobName = context.JobDetail.Key.Name,
                StartDate = DateTime.Now
            };

            await jobDataSource.Start(startDocument);

            var sw = new Stopwatch();
            sw.Start();

            _stopwatches.Add(context.FireInstanceId, sw);
        }

        public async Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException, CancellationToken cancellationToken = default)
        {
            _stopwatches[context.FireInstanceId].Stop();

            var endDocument = new JobDataEndDocument
            {
                JobId = context.FireInstanceId,
                JobType = context.JobDetail.JobType.Name,
                JobName = context.JobDetail.Key.Name,
                EndDate = DateTime.Now,
                ElapsedMilliseconds = _stopwatches[context.FireInstanceId].ElapsedMilliseconds
            };

            await jobDataSource.Finish(endDocument);

            if (jobException != null)
            {
                var logDocument = new JobDataLogDocument
                {
                    JobId = context.FireInstanceId,
                    Level = "Error",
                    JobName = context.JobDetail.Key.Name,
                    Description = jobException.ToString(),
                    CreatedDate = DateTime.Now
                };

                await jobDataSource.Log(logDocument);

                await RetryJob(context);
            }

            _stopwatches.Remove(context.FireInstanceId);
        }

        #region Helpers

        private async Task RetryJob(IJobExecutionContext context)
        {
            var hasRetry = context.MergedJobDataMap.TryGetValue("Retry", out var retry);

            if (hasRetry)
            {
                var trigger = new SimpleTriggerImpl(Guid.NewGuid().ToString());
                trigger.Description = "Retry";
                trigger.RepeatCount = 0;
                trigger.JobKey = context.JobDetail.Key;
                trigger.JobDataMap.Add("IsRetry", true);
                trigger.StartTimeUtc = DateBuilder.NextGivenMinuteDate(DateTime.Now, (int)retry);

                await context.Scheduler.ScheduleJob(trigger);
            }
        }

        #endregion
    }
}
