// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Diaggregator.Endpoints
{
    [Authorize("Admins")]
    [DescriptionMetadata("Lists all configuration entries and values")]
    public class ConfigurationEndpointHandler : DiaggregatorItem
    {
        private readonly IConfiguration _configuration;

        public ConfigurationEndpointHandler(IConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            _configuration = configuration;
        }

        public override string DisplayName => "Configuration";

        public override string Name => "configuration";

        public override string Template => null;

        public async override Task Invoke(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var info = _configuration.GetChildren().ToDictionary(c => c.Key, Convert);

            context.Response.StatusCode = 200;
            context.Response.ContentType = "application/json";

            var json = JsonConvert.SerializeObject(info);
            await context.Response.WriteAsync(json, Encoding.UTF8);
        }

        private static object Convert(IConfigurationSection section)
        {
            return (object)section.Value ?? section.GetChildren().ToDictionary(c => c.Key, Convert);
        }
    }
}