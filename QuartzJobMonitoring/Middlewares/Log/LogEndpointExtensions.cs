namespace QuartzJobMonitoring
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;

    public static class LogEndpointExtensions
    {
        public static IApplicationBuilder UseLogApiEndpoint(this IApplicationBuilder builder, PathString pathString) => builder.UseMiddleware<LogApiEndpoint>(pathString);
        public static IApplicationBuilder UseLogDashboardEndpoint(this IApplicationBuilder builder, PathString pathString) => builder.UseMiddleware<LogDashboardEndpoint>(pathString);
    }
}
