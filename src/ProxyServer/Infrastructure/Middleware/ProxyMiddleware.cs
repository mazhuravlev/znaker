// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NuGet.Configuration;
using ProxyServer.Infrastructure.Services;

namespace ProxyServer.Infrastructure.Middleware
{
    public class ProxyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ResolveProxy _resolveProxy;

        public ProxyMiddleware(RequestDelegate next, ResolveProxy resolveProxy)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            if (resolveProxy == null)
            {
                throw new ArgumentNullException(nameof(resolveProxy));
            }

            _next = next;
            _resolveProxy = resolveProxy;
            
        }

        public async Task Invoke(HttpContext context)
        {
            var httpClient = new HttpClient(new HttpClientHandler
            {
                Proxy = _resolveProxy.GetEndPointProxy(),
                UseProxy = true
            });
            var requestMessage = new HttpRequestMessage();
            if (!string.Equals(context.Request.Method, "GET", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(context.Request.Method, "HEAD", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(context.Request.Method, "DELETE", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(context.Request.Method, "TRACE", StringComparison.OrdinalIgnoreCase))
            {
                var streamContent = new StreamContent(context.Request.Body);
                requestMessage.Content = streamContent;
            }

            // Copy the request headers
            foreach (var header in context.Request.Headers)
            {
                if (!requestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray()) && requestMessage.Content != null)
                {
                    requestMessage.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
                }
            }


            var httpRequest = context.Features.Get<Microsoft.AspNetCore.Http.Features.IHttpRequestFeature>();
            requestMessage.Headers.Host = context.Request.Host.ToString();
           
            requestMessage.RequestUri = new Uri(httpRequest.RawTarget);
            requestMessage.Method = new HttpMethod(context.Request.Method);
            
            
            using (var responseMessage = await httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, context.RequestAborted))
            {
                context.Response.StatusCode = (int)responseMessage.StatusCode;
                foreach (var header in responseMessage.Headers)
                {
                    context.Response.Headers[header.Key] = header.Value.ToArray();
                }

                foreach (var header in responseMessage.Content.Headers)
                {
                    context.Response.Headers[header.Key] = header.Value.ToArray();
                }

                // SendAsync removes chunking from the response. This removes the header so it doesn't expect a chunked response.
                context.Response.Headers.Remove("transfer-encoding");
                await responseMessage.Content.CopyToAsync(context.Response.Body);
            }
        }
    }
}
