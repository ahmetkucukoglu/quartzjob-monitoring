namespace QuartzJobMonitoring
{
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json;
    using Quartz;
    using Quartz.Impl.Matchers;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class JobApiEndpoint
    {
        private readonly RequestDelegate _next;

        private readonly PathString basePath;

        public JobApiEndpoint(RequestDelegate next, PathString basePath)
        {
            _next = next;

            this.basePath = basePath;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            if (httpContext.Request.Path.StartsWithSegments(basePath.Add(JobEndpointPaths.API_PATH)))
            {
                var schedulerFactory = (ISchedulerFactory)httpContext.RequestServices.GetService(typeof(ISchedulerFactory));
                var jobDataSource = (IJobDataSource)httpContext.RequestServices.GetService(typeof(IJobDataSource));

                var response = await GetData(schedulerFactory, jobDataSource);

                httpContext.Response.ContentType = "application/json";

                await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(response));

            }
            else
            {
                await _next(httpContext);
            }
        }

        private async Task<List<JobResponse>> GetData(ISchedulerFactory schedulerFactory, IJobDataSource jobDataSource)
        {
            var response = new List<JobResponse>();

            var scheduler = await schedulerFactory.GetScheduler(default);
            var jobKeys = await scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup());

            foreach (var jobKey in jobKeys)
            {
                var job = await scheduler.GetJobDetail(jobKey);

                var history = await jobDataSource.GetLastHistoryByName(job.Key.Name);
                var statistic = await jobDataSource.GetStatisticByName(job.Key.Name);

                response.Add(new JobResponse
                {
                    Name = job.Key.Name,
                    LastRun = history != null ? TimingHelper.GetTiming(history.StartDate) : string.Empty,
                    Statistic = new StatisticResponse
                    {
                        Labels = statistic.Labels,
                        Series = statistic.Series
                    }
                });
            }

            return response;
        }
    }
}
