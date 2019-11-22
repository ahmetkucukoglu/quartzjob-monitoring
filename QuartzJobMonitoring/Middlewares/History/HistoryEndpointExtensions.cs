namespace QuartzJobMonitoring
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;

    public static class HistoryEndpointExtensions
    {
        public static IApplicationBuilder UseHistoryApiEndpoint(this IApplicationBuilder builder, PathString pathString) => builder.UseMiddleware<HistoryApiEndpoint>(pathString);
        public static IApplicationBuilder UseHistoryDashboardEndpoint(this IApplicationBuilder builder, PathString pathString) => builder.UseMiddleware<HistoryDashboardEndpoint>(pathString);
    }
}
