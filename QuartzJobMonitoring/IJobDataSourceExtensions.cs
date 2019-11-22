namespace QuartzJobMonitoring
{
    using Quartz;
    using System;
    using System.Threading.Tasks;

    public static class IJobDataSourceExtensions
    {
        public static async Task LogInformation(this IJobDataSource jobDataSource, IJobExecutionContext jobExecutionContext, string message)
        {
            await jobDataSource.Log(new JobDataLogDocument
            {
                JobId = jobExecutionContext.FireInstanceId,
                JobName = jobExecutionContext.JobDetail.Key.Name,
                Level = "Information",
                CreatedDate = DateTime.Now,
                Description = message
            });
        }

        public static async Task LogWarning(this IJobDataSource jobDataSource, IJobExecutionContext jobExecutionContext, string message)
        {
            await jobDataSource.Log(new JobDataLogDocument
            {
                JobId = jobExecutionContext.FireInstanceId,
                JobName = jobExecutionContext.JobDetail.Key.Name,
                Level = "Warning",
                CreatedDate = DateTime.Now,
                Description = message
            });
        }

        public static async Task LogError(this IJobDataSource jobDataSource, IJobExecutionContext jobExecutionContext, string message)
        {
            await jobDataSource.Log(new JobDataLogDocument
            {
                JobId = jobExecutionContext.FireInstanceId,
                JobName = jobExecutionContext.JobDetail.Key.Name,
                Level = "Error",
                CreatedDate = DateTime.Now,
                Description = message
            });
        }
    }
}
