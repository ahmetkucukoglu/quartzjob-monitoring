namespace QuartzJobMonitoring
{
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class LogApiEndpoint
    {
        private readonly RequestDelegate _next;

        private readonly PathString basePath;

        public LogApiEndpoint(RequestDelegate next, PathString basePath)
        {
            _next = next;

            this.basePath = basePath;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            if (httpContext.Request.Path.StartsWithSegments(basePath.Add(LogEndpointPaths.API_PATH)))
            {
                var jobDataSource = (IJobDataSource)httpContext.RequestServices.GetService(typeof(IJobDataSource));

                var jobId = httpContext.Request.Query["jobId"];

                var response = await GetData(jobDataSource, jobId);

                httpContext.Response.ContentType = "application/json";

                await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(response));
            }
            else
            {
                await _next(httpContext);
            }
        }

        private async Task<List<LogResponse>> GetData(IJobDataSource jobDataSource, string jobId)
        {
            var logs = await jobDataSource.GetLogsByJobId(jobId);

            var response = logs?.Select((x) => new LogResponse
            {
                CreatedDate = x.CreatedDate,
                JobName = x.JobName,
                Level = x.Level,
                Description = x.Description,
                JobId = x.JobId
            }).ToList();

            return response;
        }
    }
}
