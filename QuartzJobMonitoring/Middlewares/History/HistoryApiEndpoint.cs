namespace QuartzJobMonitoring
{
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class HistoryApiEndpoint
    {
        private readonly RequestDelegate _next;

        private readonly PathString basePath;

        public HistoryApiEndpoint(RequestDelegate next, PathString basePath)
        {
            _next = next;

            this.basePath = basePath;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            if (httpContext.Request.Path.StartsWithSegments(basePath.Add(HistoryEndpointPaths.API_PATH)))
            {
                var jobDataSource = (IJobDataSource)httpContext.RequestServices.GetService(typeof(IJobDataSource));

                var jobName = httpContext.Request.Query["jobName"];

                var response = await GetData(jobDataSource, jobName);

                httpContext.Response.ContentType = "application/json";

                await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(response));
            }
            else
            {
                await _next(httpContext);
            }
        }

        private async Task<List<HistoryResponse>> GetData(IJobDataSource jobDataSource, string jobName)
        {
            var histories = await jobDataSource.GetHistoriesByName(jobName);

            var response = histories?.Select((x) => new HistoryResponse
            {
                ElapsedMilliseconds = x.ElapsedMilliseconds,
                EndDate = x.EndDate,
                StartDate = x.StartDate,
                JobId = x.JobId,
                JobName = x.JobName,
                LastRun = TimingHelper.GetTiming(x.StartDate),
                ElapsedTime = x.EndDate.HasValue ? TimingHelper.GetTiming(x.ElapsedMilliseconds) : string.Empty
            }).ToList();

            return response;
        }
    }
}
