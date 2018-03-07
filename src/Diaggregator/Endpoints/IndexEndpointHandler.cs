// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Dispatcher;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Diaggregator.Endpoints
{
    public class IndexEndpointHandler
    {
        private readonly IEndpointCollectionProvider[] _endpointProviders;

        public IndexEndpointHandler(IEnumerable<DispatcherDataSource> dataSources)
        {
            if (dataSources == null)
            {
                throw new ArgumentNullException(nameof(dataSources));
            }
            
            _endpointProviders = dataSources.Cast<IEndpointCollectionProvider>().ToArray();
        }

        public async Task Invoke(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var endpoints = _endpointProviders
                .SelectMany(d => d.Endpoints)
                .Where(e => e.Metadata.OfType<IDiaggregatorEndpointMetadata>().Any())
                .ToArray();

            var info = new Dictionary<string, object>();

            foreach (var endpoint in endpoints)
            {
                var metadata = endpoint.Metadata.OfType<IDiaggregatorEndpointMetadata>().First();
                info.Add(metadata.ShortName, new
                {
                    DisplayName = endpoint.DisplayName,
                    Description = endpoint.Metadata.OfType<IDescriptionMetadata>().FirstOrDefault()?.Description,
                    Url = (endpoint as HttpEndpoint)?.Pattern,
                });
            }

            context.Response.StatusCode = 200;
            context.Response.ContentType = "application/json";

            var json = JsonConvert.SerializeObject(info);
            await context.Response.WriteAsync(json, Encoding.UTF8);
        }
    }
}