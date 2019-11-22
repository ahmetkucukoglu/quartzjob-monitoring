namespace QuartzJobMonitoring.Sample
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using QuartzJobMonitoring;
    using System.Collections.Generic;

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddQuartzJobMonitoring((x) =>
            {
                x.AddSchedule<MyJob>("Benim Job'ım", "0 0 3 1/1 * ? *");
                x.AddSchedule<YourJob>("Senin Job'ın", new Dictionary<string, object> { { "Deneme", 30 } }, 15, "0 0 3 1/1 * ? *");

                x.UseInMemory();
            });

            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseQuartzJobMonitoring("/quartzjob");
            app.UseMvc();
        }
    }
}
