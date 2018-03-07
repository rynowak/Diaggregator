// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Diaggregator.Endpoints
{
    [DescriptionMetadata("Lists all active logging categories")]
    public class LogsEndpointHandler : DiaggregatorItem
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly DiaggregatorLoggerProvider _loggerProvider;

        public LogsEndpointHandler(ILoggerFactory loggerFactory, DiaggregatorLoggerProvider loggerProvider)
        {
            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            if (loggerProvider == null)
            {
                throw new ArgumentNullException(nameof(loggerProvider));
            }

            _loggerFactory = loggerFactory;
            _loggerProvider = loggerProvider;
        }

        public override string DisplayName => "Log Sources";

        public override string Name => "logs";

        public override string Template => null;

        public async override Task Invoke(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var categories = _loggerProvider.GetCategoryNames();
            var levels = new Dictionary<string, string>();
            foreach (var category in categories)
            {
                levels.Add(category, LogLevel.Critical.ToString());

                var logger = _loggerFactory.CreateLogger(category);
                foreach (var level in Enum.GetValues(typeof(LogLevel)).Cast<object>().Reverse())
                {
                    if (!logger.IsEnabled((LogLevel)level))
                    {
                        break;
                    }

                    levels[category] = level.ToString();
                }
            }

            var json = JsonConvert.SerializeObject(levels);

            context.Response.StatusCode = 200;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(json, Encoding.UTF8);
        }
    }
}