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
    public class IndexEndpointHandler : DiaggregatorItem
    {
        public override string DisplayName => "Diaggregator Info";

        public override string Name => "index";

        public override string Template => null;

        public async override Task Invoke(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var endpointProviders = context.RequestServices.GetRequiredService<IEnumerable<DispatcherDataSource>>().Cast<IEndpointCollectionProvider>().ToArray();
            var endpoints = endpointProviders
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