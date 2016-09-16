using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabber2.Infrastructure;
using Grabber2.Infrastructure.Components.AvitoRu;
using Grabber2.Infrastructure.Components.Olx;
using Grabber2.Infrastructure.Components.ProxyManager;
using Grabber2.Infrastructure.Services;
using Grabber2.Infrastructure.Services.Configuration;
using Grabber2.Infrastructure.Services.Logging;
using Grabber2.Infrastructure.Services.Network;
using Grabber2.Infrastructure.Services.Server;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PostgreSqlProvider;

namespace Grabber2
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
            services.AddDbContext<GrabberContext>(c => c.UseNpgsql(Configuration["ConnectionStrings:GrabberConnectionString"]), ServiceLifetime.Transient);
            services.AddDbContext<ZnakerContext>(c => c.UseNpgsql(Configuration["ConnectionStrings:ZnakerConnectionString"]), ServiceLifetime.Transient);

            services.AddSingleton<ServerService>();
            services.AddSingleton<NetworkService>();
            services.AddSingleton<LoggingService>();
            services.AddSingleton<ConfigurationService>();

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


            var server = provider.GetService<ServerService>();
            server.RegisterComponent(new OlxUa());
            server.RegisterComponent(new AviroRu());
            server.RegisterComponent(new ProxyManager());
            server.Start();
        }
    }
}
