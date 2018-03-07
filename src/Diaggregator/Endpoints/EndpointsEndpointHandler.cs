// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Dispatcher;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Diaggregator.Endpoints
{
    public class EndpointsEndpointHandler
    {
        private readonly DispatcherDataSource[] _dataSources;

        public EndpointsEndpointHandler(IEnumerable<DispatcherDataSource> dataSources)
        {
            if (dataSources == null)
            {
                throw new ArgumentNullException(nameof(dataSources));
            }

            _dataSources = dataSources.ToArray();
        }
        
        public async Task Invoke(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var endpoints = _dataSources
                .Cast<IEndpointCollectionProvider>()
                .SelectMany(d => d.Endpoints)
                .Select(e => new 
                { 
                    DisplayName = e.DisplayName,
                    Url = ((HttpEndpoint)e).Pattern,
                    Metadata = e.Metadata.ToArray(), 
                })
                .ToArray();

            var json = JsonConvert.SerializeObject(endpoints);

            context.Response.StatusCode = 200;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(json, Encoding.UTF8);
        }
    }
}