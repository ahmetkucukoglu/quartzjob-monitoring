namespace QuartzJobMonitoring
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;

    public static class ScheduleEndpointExtensions
    {
        public static IApplicationBuilder UseScheduleApiEndpoint(this IApplicationBuilder builder, PathString pathString) => builder.UseMiddleware<ScheduleApiEndpoint>(pathString);
        public static IApplicationBuilder UseScheduleDashboardEndpoint(this IApplicationBuilder builder, PathString pathString) => builder.UseMiddleware<ScheduleDashboardEndpoint>(pathString);
    }
}
