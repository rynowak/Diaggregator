// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Diaggregator;
using Microsoft.AspNetCore.Dispatcher;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DiaggregatorServiceCollectionExtensions
    {
        public static IServiceCollection AddDiaggregator(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddSingleton<DispatcherDataSource, DiaggregatorDataSource>();
            
            services.AddSingleton<EndpointsEndpointHandler>();

            services.AddSingleton<LogEndpointHandler>();

            services.AddSingleton<DiaggregatorLoggerProvider>();
            services.AddSingleton<ILoggerProvider>(s => s.GetRequiredService<DiaggregatorLoggerProvider>());
            services.AddSingleton<LogsEndpointHandler>();
            
            return services;
        }
    }
}