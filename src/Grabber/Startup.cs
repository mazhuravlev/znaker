using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Grabber.Grabbers.Olx;
using Grabber.Infrastructure;
using Grabber.Infrastructure.Http;
using Grabber.Infrastructure.Managers;
using Grabber.Infrastructure.Services;
using Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Grabber
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ISitemapManager, SitemapManager>();
            services.AddSingleton<IAdvertManager, AdvertManager>();
            services.AddSingleton<IAdvertService, AdvertService>();
            services.AddSingleton<ISitemapService, SitemapService>();
            services.AddSingleton<IProxyService, ProxyService>();
            services.AddSingleton<SimpleNadproxy>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory,
            IServiceProvider provider, IApplicationLifetime appLifetime)
        {
            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var olxUaConfig = new OlxConfig(
                OlxType.Ua,
                "http://olx.ua/sitemap.xml",
                "https://ssl.olx.ua/i2/obyavlenie/?json=1&id={0}&version=2.3.2",
                "https://ssl.olx.ua/i2/ajax/ad/getcontact/?type=phone&json=1&id={0}&version=2.3.2"
            );

            var sitemapManager = provider.GetService<ISitemapManager>();
            var sitemapGrabber = new OlxSitemapGrabber(olxUaConfig, new PlainHttpClient());
            sitemapManager.AddGrabber(SourceType.OlxUa.ToString(), sitemapGrabber, isEnabled: true);
            sitemapManager.Run(appLifetime.ApplicationStopping);

            var nadproxy = provider.GetService<SimpleNadproxy>();
            nadproxy.AddProxy(new Proxy {Host = "127.0.0.1", Port = 3128});
            nadproxy.AddProxy(new Proxy {Host = "127.0.0.1", Port = 3128});
            nadproxy.AddProxy(new Proxy {Host = "46.101.22.147", Port = 8118 });
            var adManager = provider.GetService<IAdvertManager>();
            var grabber = new OlxAdvertGrabber(olxUaConfig, nadproxy);
            adManager.AddGrabber(SourceType.OlxUa.ToString(), grabber);
            adManager.Run(appLifetime.ApplicationStopping);

            app.Run(async (context) => { await context.Response.WriteAsync("Hello World!"); });
        }
    }
}