// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Dispatcher;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Diaggregator
{
    public class DiaggregatorDataSource : DefaultDispatcherDataSource
    {
        public DiaggregatorDataSource()
        {
            Endpoints.Add(new RoutePatternEndpoint("/info", async (context) =>
            {
                var info = new Dictionary<string, string>()
               {
                   { "logs", "/logs" },
               };

                var json = JsonConvert.SerializeObject(info);

                context.Response.StatusCode = 200;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsync(json, Encoding.UTF8);
            }));
        }
    }
}