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

    public class HistoryDashboardEndpoint
    {
        private readonly RequestDelegate _next;

        private readonly PathString basePath;

        public HistoryDashboardEndpoint(RequestDelegate next, PathString basePath)
        {
            _next = next;

            this.basePath = basePath;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            if (httpContext.Request.Path.StartsWithSegments(basePath.Add(HistoryEndpointPaths.DASHBOARD_PATH)))
            {
                var jobDataSource = (IJobDataSource)httpContext.RequestServices.GetService(typeof(IJobDataSource));

                var jobName = httpContext.Request.Query["jobName"];

                var response = await GetData(jobDataSource, jobName);

                var razorViewEngine = (IRazorViewEngine)httpContext.RequestServices.GetService(typeof(IRazorViewEngine));
                var tempDataProvider = (ITempDataProvider)httpContext.RequestServices.GetService(typeof(ITempDataProvider));

                var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());

                var path = "\\Middlewares\\History\\Models\\Histories.cshtml";
                var view = razorViewEngine.GetView(null, path, true).View;

                var html = string.Empty;

                using (var output = new StringWriter())
                {
                    var viewData = new ViewDataDictionary<List<HistoryResponse>>(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                    {
                        Model = response
                    };

                    viewData.Add("JobPath", basePath.Add(JobEndpointPaths.DASHBOARD_PATH));
                    viewData.Add("LogPath", basePath.Add(LogEndpointPaths.DASHBOARD_PATH));

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
