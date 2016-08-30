using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OlxLib;
using Microsoft.EntityFrameworkCore;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.PostgreSql.NetCore;
using OlxLib.Workers;
using PostgreSqlProvider;

namespace OlxServer
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHangfire(x =>
            {
                x.UseStorage(new PostgreSqlStorage(Configuration["ConnectionStrings:ParserConnectionString"]));
                RecurringJob.AddOrUpdate<TestWorker>(z => z.Run(), Cron.Minutely);
            });

            
            services.AddDbContext<ParserContext>(c => c.UseNpgsql(Configuration["ConnectionStrings:ParserConnectionString"]), ServiceLifetime.Transient);
            services.AddDbContext<ZnakerContext>(c => c.UseNpgsql(Configuration["ConnectionStrings:ZnakerConnectionString"]), ServiceLifetime.Transient);

            services.AddSingleton<TestWorker>();

            // Add framework services.
            services.AddMvc();
            services.AddRouting();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseHangfireServer(new BackgroundJobServerOptions
            {
                ServerName = "localServer",
                WorkerCount = Environment.ProcessorCount * 5
            });
            app.UseHangfireDashboard(options: new DashboardOptions
            {
                StatsPollingInterval = 10000,
                Authorization = new List<IDashboardAuthorizationFilter>()
            });


            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseDeveloperExceptionPage();

            //if (env.IsDevelopment()) { } else { }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
