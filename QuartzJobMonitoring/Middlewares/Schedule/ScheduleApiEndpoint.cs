namespace QuartzJobMonitoring
{
    using CronExpressionDescriptor;
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json;
    using Quartz;
    using Quartz.Impl.Matchers;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class ScheduleApiEndpoint
    {
        private readonly RequestDelegate _next;

        private readonly PathString basePath;

        public ScheduleApiEndpoint(RequestDelegate next, PathString basePath)
        {
            _next = next;

            this.basePath = basePath;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            if (httpContext.Request.Path.StartsWithSegments(basePath.Add(ScheduleEndpointPaths.API_PATH)))
            {
                var schedulerFactory = (ISchedulerFactory)httpContext.RequestServices.GetService(typeof(ISchedulerFactory));

                var jobName = httpContext.Request.Query["jobName"];

                var response = await GetData(schedulerFactory, jobName);

                httpContext.Response.ContentType = "application/json";

                await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(response));
            }
            else
            {
                await _next(httpContext);
            }
        }

        private async Task<List<ScheduleResponse>> GetData(ISchedulerFactory schedulerFactory, string jobName)
        {
            var response = new List<ScheduleResponse>();

            var scheduler = await schedulerFactory.GetScheduler(default);
            var jobKeys = await scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup());

            foreach (var jobKey in jobKeys)
            {
                var job = await scheduler.GetJobDetail(jobKey);

                if (job.Key.Name == jobName)
                {
                    var triggers = await scheduler.GetTriggersOfJob(jobKey);

                    var schedule = new ScheduleResponse
                    {
                        JobName = jobName
                    };

                    foreach (var trigger in triggers)
                    {
                        if (trigger.JobDataMap.TryGetValue("IsRetry", out var isRetry))
                            continue;

                        schedule.Cron.Add(new ScheduleCronResponse
                        {
                            Expression = trigger.Description,
                            Description = ExpressionDescriptor.GetDescription(trigger.Description, new Options { Locale = "tr-TR" })
                        });
                    }

                    response.Add(schedule);

                    break;
                }
            }

            return response;
        }
    }
}
