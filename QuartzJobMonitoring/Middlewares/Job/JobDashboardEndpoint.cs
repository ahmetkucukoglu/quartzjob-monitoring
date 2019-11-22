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
    using Quartz;
    using Quartz.Impl.Matchers;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    public class JobDashboardEndpoint
    {
        private readonly RequestDelegate _next;

        private readonly PathString basePath;

        public JobDashboardEndpoint(RequestDelegate next, PathString basePath)
        {
            _next = next;

            this.basePath = basePath;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            if (httpContext.Request.Path.StartsWithSegments(basePath.Add(JobEndpointPaths.DASHBOARD_PATH)))
            {
                var schedulerFactory = (ISchedulerFactory)httpContext.RequestServices.GetService(typeof(ISchedulerFactory));
                var jobDataSource = (IJobDataSource)httpContext.RequestServices.GetService(typeof(IJobDataSource));

                var response = await GetData(schedulerFactory, jobDataSource);

                var razorViewEngine = (IRazorViewEngine)httpContext.RequestServices.GetService(typeof(IRazorViewEngine));
                var tempDataProvider = (ITempDataProvider)httpContext.RequestServices.GetService(typeof(ITempDataProvider));

                var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
                
                var path = "\\Middlewares\\Job\\Models\\Jobs.cshtml";
                var view = razorViewEngine.GetView(null, path, true).View;

                var html = string.Empty;

                using (var output = new StringWriter())
                {
                    var viewData = new ViewDataDictionary<List<JobResponse>>(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                    {
                        Model = response
                    };

                    viewData.Add("SchedulePath", basePath.Add(ScheduleEndpointPaths.DASHBOARD_PATH));
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
