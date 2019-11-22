namespace QuartzJobMonitoring
{
    using Microsoft.AspNetCore.Http;
    using Quartz;
    using Quartz.Impl.Matchers;
    using System.Linq;
    using System.Threading.Tasks;

    public class TriggerApiEndpoint
    {
        private readonly RequestDelegate _next;

        private readonly PathString basePath;
        
        public TriggerApiEndpoint(RequestDelegate next, PathString pathString)
        {
            _next = next;

            this.basePath = pathString;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            if (httpContext.Request.Path.StartsWithSegments(basePath.Add(TriggerEndpointPaths.API_PATH)))
            {
                var schedulerFactory = (ISchedulerFactory)httpContext.RequestServices.GetService(typeof(ISchedulerFactory));

                var jobName = httpContext.Request.Query["jobName"];
                var isRedirect = httpContext.Request.Query["isRedirect"];

                var scheduler = await schedulerFactory.GetScheduler(default);
                var jobKeys = await scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup());

                var selectedJobKey = jobKeys.FirstOrDefault((x) => x.Name == jobName);

                if (selectedJobKey != null)
                {
                    await scheduler.TriggerJob(selectedJobKey);

                    if(!string.IsNullOrEmpty(isRedirect))
                    {
                        var jobsPath = basePath.Add(JobEndpointPaths.DASHBOARD_PATH);

                        httpContext.Response.Redirect(jobsPath);
                    }
                    else
                    {
                        httpContext.Response.StatusCode = 200;
                    }
                }
                else
                {

                    await scheduler.TriggerJob(selectedJobKey);

                    httpContext.Response.StatusCode = 404;
                }
            }
            else
            {
                await _next(httpContext);
            }
        }
    }
}
