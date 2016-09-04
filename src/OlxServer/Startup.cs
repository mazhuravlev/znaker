using System;
using System.Collections.Generic;
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
            });

            services.AddDbContext<ParserContext>(c => c.UseNpgsql(Configuration["ConnectionStrings:ParserConnectionString"]), ServiceLifetime.Transient);
            services.AddDbContext<ZnakerContext>(c => c.UseNpgsql(Configuration["ConnectionStrings:ZnakerConnectionString"]), ServiceLifetime.Transient);

            services.AddTransient<ExportManager>();
            services.AddTransient<SitemapWorker>();
            services.AddSingleton<DownloadManager>();



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
                WorkerCount = Environment.ProcessorCount * 5,
                Queues = new []{ "sitemap_download", "export_manager", "download_manager", "download_worker" }
            });
            app.UseHangfireDashboard(options: new DashboardOptions
            {
                StatsPollingInterval = 10000,
                Authorization = new List<IDashboardAuthorizationFilter>()
            });



            //run jobs
            if (env.IsDevelopment())
            {
                RecurringJob.AddOrUpdate<ExportManager>(z => z.RunExport(JobCancellationToken.Null, 500), Cron.Yearly);
                RecurringJob.AddOrUpdate<ExportManager>(z => z.RunCleaner(7), Cron.Yearly);

                RecurringJob.AddOrUpdate<DownloadManager>(z => z.Run(OlxType.Ua, JobCancellationToken.Null), Cron.Yearly);

                RecurringJob.AddOrUpdate<SitemapWorker>(z => z.Run(OlxType.Ua), Cron.Yearly);
            }
            else
            {
                RecurringJob.AddOrUpdate<ExportManager>(z => z.RunExport(JobCancellationToken.Null, 500), Cron.Minutely);
                RecurringJob.AddOrUpdate<ExportManager>(z => z.RunCleaner(7), Cron.HourInterval(12));

                BackgroundJob.Enqueue<DownloadManager>(z => z.Run(OlxType.Ua, JobCancellationToken.Null));

                RecurringJob.AddOrUpdate<SitemapWorker>(z => z.Run(OlxType.Ua), Cron.HourInterval(6));
            }


            //BackgroundJob.Enqueue<SitemapWorker>(z => z.Run(OlxType.Ua));

            /*RecurringJob.AddOrUpdate<SitemapWorker>(z => z.Run(OlxType.Kz), Cron.HourInterval(8));
            BackgroundJob.Schedule<SitemapWorker>(z => z.Run(OlxType.Kz), TimeSpan.FromMinutes(5));

            RecurringJob.AddOrUpdate<SitemapWorker>(z => z.Run(OlxType.Uz), Cron.HourInterval(8));
            BackgroundJob.Schedule<SitemapWorker>(z => z.Run(OlxType.Uz), TimeSpan.FromMinutes(10));

            RecurringJob.AddOrUpdate<SitemapWorker>(z => z.Run(OlxType.By), Cron.HourInterval(8));
            BackgroundJob.Schedule<SitemapWorker>(z => z.Run(OlxType.By), TimeSpan.FromMinutes(15));*/





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
