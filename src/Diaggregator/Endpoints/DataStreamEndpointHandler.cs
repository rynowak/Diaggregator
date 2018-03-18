// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Text;
using System.Threading.Tasks;
using Diaggregator.DataStreams;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Diaggregator.Endpoints
{
    public class DataStreamEndpointHandler : DiaggregatorItem
    {
        private readonly DataStreamRegistry _registry;

        public DataStreamEndpointHandler(DataStreamRegistry registry, string name, string displayName, string template)
        {
            _registry = registry;

            Name = name;
            DisplayName = displayName;
            Template = template;
        }
        public override string DisplayName { get; }

        public override string Name { get; }

        public override string Template { get; }

        public override async Task Invoke(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            _registry.CurrentValues.TryGetValue(Name, out var value);
            value = value ?? new { };          

            context.Response.StatusCode = 200;
            context.Response.ContentType = "application/json";

            var json = JsonConvert.SerializeObject(value);
            await context.Response.WriteAsync(json, Encoding.UTF8);
        }
    }
}