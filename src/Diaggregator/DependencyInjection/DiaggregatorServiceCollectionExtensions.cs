// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Diaggregator;
using Diaggregator.DataStreams;
using Diaggregator.Endpoints;
using Diaggregator.Mvc;
using GraphQL;
using GraphQL.Http;
using Microsoft.AspNetCore.Dispatcher;
using Microsoft.Extensions.Hosting;
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

            services.AddSingleton<IHostedService, DiaggregatorHostedService>();
            services.AddSingleton<DataStreamRegistry, DefaultDataStreamRegistry>();

            services.AddSingleton<IDataStreamLifetime, ConfigurationDataStream>();

            services.AddSingleton<DispatcherDataSource, DiaggregatorDataSource>();
            services.AddSingleton<DispatcherDataSource, MvcDispatcherDataSource>();
            
            services.AddSingleton<DiaggregatorLoggerProvider>();
            services.AddSingleton<ILoggerProvider>(s => s.GetRequiredService<DiaggregatorLoggerProvider>());

            services.AddSingleton<DiaggregatorItem, IndexEndpointHandler>();
            services.AddSingleton<DiaggregatorItem>((s) =>
            {
                var registry = s.GetRequiredService<DataStreamRegistry>();
                return new DataStreamEndpointHandler(registry, "configuration", "Configuration", "configuration");
            });
            services.AddSingleton<DiaggregatorItem, EndpointsEndpointHandler>();
            services.AddSingleton<DiaggregatorItem, GraphQLEndpointHandler>();
            services.AddSingleton<DiaggregatorItem, LogStreamEndpointHandler>();
            services.AddSingleton<DiaggregatorItem, LogsEndpointHandler>();

            // GraphQL
            services.AddSingleton<IDocumentExecuter, DocumentExecuter>();
            services.AddSingleton<IDocumentWriter>(new DocumentWriter(indent: true));

            return services;
        }
    }
}