namespace QuartzJobMonitoring
{
    using CronExpressionDescriptor;
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

    public class ScheduleDashboardEndpoint
    {
        private readonly RequestDelegate _next;

        private readonly PathString basePath;

        public ScheduleDashboardEndpoint(RequestDelegate next, PathString basePath)
        {
            _next = next;

            this.basePath = basePath;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            if (httpContext.Request.Path.StartsWithSegments(basePath.Add(ScheduleEndpointPaths.DASHBOARD_PATH)))
            {
                var schedulerFactory = (ISchedulerFactory)httpContext.RequestServices.GetService(typeof(ISchedulerFactory));

                var jobName = httpContext.Request.Query["jobName"];

                var response = await GetData(schedulerFactory, jobName);

                var razorViewEngine = (IRazorViewEngine)httpContext.RequestServices.GetService(typeof(IRazorViewEngine));
                var tempDataProvider = (ITempDataProvider)httpContext.RequestServices.GetService(typeof(ITempDataProvider));

                var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());

                var path = "\\Middlewares\\Schedule\\Models\\Schedules.cshtml";
                var view = razorViewEngine.GetView(null, path, true).View;

                var html = string.Empty;

                using (var output = new StringWriter())
                {
                    var viewData = new ViewDataDictionary<List<ScheduleResponse>>(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                    {
                        Model = response
                    };

                    viewData.Add("JobPath", basePath.Add(JobEndpointPaths.DASHBOARD_PATH));
                    viewData.Add("TriggerPath", basePath.Add(TriggerEndpointPaths.API_PATH));

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
