namespace QuartzJobMonitoring
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;

    public static class TriggerEndpointExtensions
    {
        public static IApplicationBuilder UseTriggerApiEndpoint(this IApplicationBuilder builder, PathString pathString) => builder.UseMiddleware<TriggerApiEndpoint>(pathString);
    }
}
