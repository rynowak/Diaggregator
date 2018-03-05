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

namespace Diaggregator
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
        
        public async Task InvokeAsync(HttpContext httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            var endpoints = _dataSources
                .Cast<IEndpointCollectionProvider>()
                .SelectMany(d => d.Endpoints)
                .Select(e => new 
                { 
                    DisplayName = e.DisplayName,
                    Url = ((RoutePatternEndpoint)e).Pattern,
                    Metadata = e.Metadata.ToArray(), 
                })
                .ToArray();

            var json = JsonConvert.SerializeObject(endpoints);

            httpContext.Response.StatusCode = 200;
            httpContext.Response.ContentType = "application/json";

            await httpContext.Response.WriteAsync(json, Encoding.UTF8);
        }
    }
}