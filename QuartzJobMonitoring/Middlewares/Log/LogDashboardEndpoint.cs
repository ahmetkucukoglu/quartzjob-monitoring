namespace QuartzJobMonitoring
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Abstractions;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.AspNetCore.Mvc.Razor;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Microsoft.AspNetCore.Routing;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    public class LogDashboardEndpoint
    {
        private readonly RequestDelegate _next;

        private readonly PathString basePath;

        public LogDashboardEndpoint(RequestDelegate next, PathString basePath)
        {
            _next = next;
            this.basePath = basePath;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            if (httpContext.Request.Path.StartsWithSegments(basePath.Add(LogEndpointPaths.DASHBOARD_PATH)))
            {
                var jobDataSource = (IJobDataSource)httpContext.RequestServices.GetService(typeof(IJobDataSource));

                var jobId = httpContext.Request.Query["jobId"];

                var response = await GetData(jobDataSource, jobId);

                var razorViewEngine = (IRazorViewEngine)httpContext.RequestServices.GetService(typeof(IRazorViewEngine));
                var tempDataProvider = (ITempDataProvider)httpContext.RequestServices.GetService(typeof(ITempDataProvider));

                var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());

                var path = "\\Middlewares\\Log\\Models\\Logs.cshtml";
                var view = razorViewEngine.GetView(null, path, true).View;

                var html = string.Empty;

                using (var output = new StringWriter())
                {
                    var viewData = new ViewDataDictionary<List<LogResponse>>(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                    {
                        Model = response
                    };

                    viewData.Add("HistoryPath", basePath.Add(HistoryEndpointPaths.DASHBOARD_PATH));

                    var viewContext = new ViewContext(
                      actionContext,
                      view,
                      viewData,
                      new TempDataDictionary(actionContext.HttpContext, tempDataProvider),
                      output,
                      new HtmlHelperOptions());


                    await view.RenderAsync(viewContext);

                    html = output.ToString();
                }

                httpContext.Response.ContentType = "text/html";

                await httpContext.Response.WriteAsync(html);
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
