using System;
using Microsoft.AspNetCore.Builder;

namespace ProxyServer.Infrastructure.Middleware
{
    public static class ProxyExtension
    {
        public static IApplicationBuilder RunProxy(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseMiddleware<ProxyMiddleware>();
        }
    }
}
