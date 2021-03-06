// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Dispatcher;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Diaggregator.Endpoints
{
    //[Authorize("Members")]
    [DescriptionMetadata("Lists all routeable endpoints in the application")]
    public class EndpointsEndpointHandler : DiaggregatorItem
    {
        public override string DisplayName => "Endpoints";

        public override string Name => "endpoints";

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
                .Select(e => new 
                { 
                    DisplayName = e.DisplayName,
                    Url = ((HttpEndpoint)e).Pattern,
                    Metadata = e.Metadata.Select(m => new MetadataItem(m)).ToArray(), 
                })
                .ToArray();

            var json = JsonConvert.SerializeObject(endpoints);

            context.Response.StatusCode = 200;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(json, Encoding.UTF8);
        }

        private class MetadataItem
        {
            public MetadataItem(object item)
            {
                Item = item;

                Type = item.GetType().FullName;
                Interfaces = item.GetType().GetInterfaces().Select(i => i.FullName).ToArray();
            }

            public object Item { get; set; }

            public string Type { get; set; }

            public string[] Interfaces { get; set; }
        }
    }
}