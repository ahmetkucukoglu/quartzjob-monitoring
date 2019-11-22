namespace QuartzJobMonitoring
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;

    public static class JobEndpointExtensions
    {
        public static IApplicationBuilder UseJobApiEndpoint(this IApplicationBuilder builder, PathString pathString) => builder.UseMiddleware<JobApiEndpoint>(pathString);
        public static IApplicationBuilder UseJobDashboardEndpoint(this IApplicationBuilder builder, PathString pathString) => builder.UseMiddleware<JobDashboardEndpoint>(pathString);
    }
}
