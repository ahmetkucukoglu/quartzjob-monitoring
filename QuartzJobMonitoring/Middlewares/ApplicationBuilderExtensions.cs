namespace QuartzJobMonitoring
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;

    public static class ApplicationBuilderExtensions
    {
        public static void UseQuartzJobMonitoring(this IApplicationBuilder applicationBuilder, PathString pathString)
        {
            applicationBuilder.UseHistoryApiEndpoint(pathString)
                .UseHistoryDashboardEndpoint(pathString);
            
            applicationBuilder.UseJobApiEndpoint(pathString)
                .UseJobDashboardEndpoint(pathString);

            applicationBuilder.UseLogApiEndpoint(pathString)
                .UseLogDashboardEndpoint(pathString);

            applicationBuilder.UseScheduleApiEndpoint(pathString)
                .UseScheduleDashboardEndpoint(pathString);

            applicationBuilder.UseTriggerApiEndpoint(pathString);
        }
    }
}
