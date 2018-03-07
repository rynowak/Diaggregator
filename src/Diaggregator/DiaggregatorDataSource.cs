// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Diaggregator.Endpoints;
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
            Endpoints.Add(new HttpEndpoint(
                "/", 
                new { },
                "GET",
                async (context) =>
                {
                    var handler = context.RequestServices.GetRequiredService<IndexEndpointHandler>();
                    await handler.Invoke(context);
                },
                "Diaggregator Info"));

            Endpoints.Add(new HttpEndpoint(
                "/configuration",
                new { },
                "GET",
                async (context) =>
                {
                    var handler = context.RequestServices.GetRequiredService<ConfigurationEndpointHandler>();
                    await handler.Invoke(context);
                },
                "Configuration",
                new DiaggregatorEndpointMetadata("configuration"),
                new DescriptionMetadata("Lists all configuration entries and values")));

            Endpoints.Add(new HttpEndpoint(
                "/endpoints",
                new { },
                "GET",
                async (context) =>
                {
                    var handler = context.RequestServices.GetRequiredService<EndpointsEndpointHandler>();
                    await handler.Invoke(context);
                },
                "Endpoints",
                new DiaggregatorEndpointMetadata("endpoints"),
                new DescriptionMetadata("Lists all routeable endpoints in the application")));

            Endpoints.Add(new HttpEndpoint(
                "/log/{category}",
                new { },
                "GET", 
                async (context) =>
                {
                    var handler = context.RequestServices.GetRequiredService<LogStreamEndpointHandler>();
                    await handler.Invoke(context);
                },
                "Streaming Logs",
                new DiaggregatorEndpointMetadata("logstream"),
                new DescriptionMetadata("Streams log messages")));

            Endpoints.Add(new HttpEndpoint(
                "/logs",
                new { },
                "GET",
                async (context) =>
                {
                    var handler = context.RequestServices.GetRequiredService<LogsEndpointHandler>();
                    await handler.Invoke(context);
                },
                "Log Sources",
                new DiaggregatorEndpointMetadata("logs"),
                new DescriptionMetadata("Lists all active logging categories")));
        }
    }
}