// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Diaggregator;
using Microsoft.AspNetCore.Dispatcher;

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
            return services;
        }
    }
}