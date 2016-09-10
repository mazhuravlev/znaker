using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using GrabberServer.Grabbers;
using GrabberServer.Grabbers.Managers;
using GrabberServer.Grabbers.Nadproxy;
using GrabberServer.Grabbers.Olx;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Razor.Parser;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PostgreSqlProvider;
using Microsoft.EntityFrameworkCore;

namespace GrabberServer
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
            services.AddDbContext<GrabberContext>(
                c => c.UseNpgsql(Configuration["ConnectionStrings:GrabberConnectionString"]), ServiceLifetime.Transient);
            services.AddDbContext<ZnakerContext>(
                c => c.UseNpgsql(Configuration["ConnectionStrings:ZnakerConnectionString"]), ServiceLifetime.Transient);

            services.AddSingleton<SitemapGrabberManager>();
            services.AddSingleton<AdGrabberManager>();
            services.AddTransient<IAdJobsService, AdJobsService>();
            services.AddTransient<ISitemapService, SitemapService>();


            // Add framework services.
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IServiceProvider provider)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            var olxUaConfig = new OlxConfig(OlxType.Ua, "http://olx.ua/sitemap.xml", "PlaceHereAdvdataurl{0}",
                "PlaceHereAdvcontacturl{0}");
            var sitemapManager = provider.GetService<SitemapGrabberManager>();
            var sitemapGrabber = new OlxSitemapGrabber(olxUaConfig, new GrabberHttpClient());
            sitemapManager.AddGrabber("olx_ua", sitemapGrabber);

            var adManager = provider.GetService<AdGrabberManager>();
            var adGrabber = new OlxAdGrabber(olxUaConfig, new GrabberHttpClient());
            adManager.AddGrabber("olx_ua", adGrabber);

            sitemapManager.Run(CancellationToken.None);
            adManager.Run(CancellationToken.None);
        }
    }
}