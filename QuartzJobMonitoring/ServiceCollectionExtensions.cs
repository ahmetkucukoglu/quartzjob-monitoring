namespace QuartzJobMonitoring
{
    using Quartz;
    using Quartz.Impl;
    using Quartz.Spi;
    using Microsoft.Extensions.DependencyInjection;
    using System;

    public static class ServiceCollectionExtensions
    {
        public static void AddQuartzJobMonitoring(this IServiceCollection services, Action<QuartzJobMonitoringOptions> setupAction)
        {
            services.AddSingleton<IJobFactory, JobFactory>();
            services.AddTransient<ISchedulerFactory, StdSchedulerFactory>();
            services.AddTransient<IJobListener, JobListener>();

            services.AddHostedService<JobHostedService>();

            var options = new QuartzJobMonitoringOptions(services);

            setupAction(options);
        }
    }
}
