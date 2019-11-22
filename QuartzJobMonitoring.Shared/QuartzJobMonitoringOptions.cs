namespace QuartzJobMonitoring
{
    using Microsoft.Extensions.DependencyInjection;
    using Quartz;
    using System.Collections.Generic;

    public class QuartzJobMonitoringOptions
    {
        public readonly IServiceCollection services;

        public QuartzJobMonitoringOptions(IServiceCollection services)
        {
            this.services = services;
        }

        public void AddSchedule<T>(string jobName, params string[] cronExpressions) where T: IJob
        {
            services.AddTransient(typeof(T));

            services.AddSingleton(new JobSchedule(
                    jobType: typeof(T),
                    jobName: jobName,
                    cronExpressions: cronExpressions,
                    data: null,
                    retry: default));
        }

        public void AddSchedule<T>(string jobName, Dictionary<string, object> data, params string[] cronExpressions) where T : IJob
        {
            services.AddTransient(typeof(T));

            services.AddSingleton(new JobSchedule(
                    jobType: typeof(T),
                    jobName: jobName,
                    cronExpressions: cronExpressions,
                    data: data,
                    retry: default));
        }

        public void AddSchedule<T>(string jobName, Dictionary<string, object> data, int retry, params string[] cronExpressions) where T : IJob
        {
            services.AddTransient(typeof(T));

            services.AddSingleton(new JobSchedule(
                    jobType: typeof(T),
                    jobName: jobName,
                    cronExpressions: cronExpressions,
                    data: data,
                    retry: retry));
        }
    }
}
