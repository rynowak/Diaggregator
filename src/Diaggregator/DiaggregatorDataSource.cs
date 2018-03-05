// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Dispatcher;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Diaggregator
{
    public class DiaggregatorDataSource : DefaultDispatcherDataSource
    {
        public DiaggregatorDataSource()
        {
            Endpoints.Add(new RoutePatternEndpoint("/", async (httpContext) =>
            {
                var dataSources = httpContext.RequestServices.GetRequiredService<IEnumerable<DispatcherDataSource>>();
                var endpoints = dataSources
                    .Cast<IEndpointCollectionProvider>()
                    .SelectMany(d => d.Endpoints)
                    .Where(e => e.Metadata.OfType<IDiaggregatorEndpointMetadata>().Any())
                    .ToArray();

                var info = new Dictionary<string, object>();

                foreach (var endpoint in endpoints)
                {
                    var metadata = endpoint.Metadata.OfType<IDiaggregatorEndpointMetadata>().First();
                    info.Add(metadata.ShortName, new 
                    { 
                        DisplayName = metadata.DisplayName,
                        Description = metadata.Description,
                        Url = ((RoutePatternEndpoint)endpoint).Pattern,
                    });
                }

                var json = JsonConvert.SerializeObject(info);

                httpContext.Response.StatusCode = 200;
                httpContext.Response.ContentType = "application/json";

                await httpContext.Response.WriteAsync(json, Encoding.UTF8);
            }));

            Endpoints.Add(new RoutePatternEndpoint(
                "/endpoints", 
                async (httpContex) =>
                {
                    var handler = httpContex.RequestServices.GetRequiredService<EndpointsEndpointHandler>();
                    await handler.InvokeAsync(httpContex);
                }, 
                new DiaggregatorEndpointMetadata()
                { 
                    DisplayName = "Endpoints",
                    Description = "Lists all routeable endpoints in the application",
                    ShortName = "endpoints",
                }));

            Endpoints.Add(new RoutePatternEndpoint(
                "/logs", 
                async (httpContex) =>
                {
                    var handler = httpContex.RequestServices.GetRequiredService<LogsEndpointHandler>();
                    await handler.InvokeAsync(httpContex);
                }, 
                new DiaggregatorEndpointMetadata()
                { 
                    DisplayName = "Logs",
                    Description = "Lists all active logging categories",
                    ShortName = "logs",
                }));
        }
    }
}