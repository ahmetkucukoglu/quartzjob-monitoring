namespace QuartzJobMonitoring.Test
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddQuartzJobMonitoring((x) =>
            {
                x.AddSchedule<TestJob>("TestJob", "0 0 3 1/1 * ? *");

                x.UseInMemory();
            });

            services.AddMvcCore();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseQuartzJobMonitoring("/quartzjob");
            app.UseMvc();
        }
    }
}
