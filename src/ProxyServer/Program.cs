using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace ProxyServer
{
    public class Program
    {
        public const int ProxyPort = 5020;
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel(c =>
                {
                    //c.UseConnectionLogging("KESTREL");
                })
                .UseUrls($"http://localhost:{ProxyPort}", "http://localhost:5030")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
