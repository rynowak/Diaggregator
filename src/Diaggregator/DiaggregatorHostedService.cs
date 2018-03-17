// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Diaggregator
{
    internal class DiaggregatorHostedService : IHostedService
    {
        private readonly DataStreamRegistry _registry;
        private readonly IDataStreamLifetime[] _items;

        public DiaggregatorHostedService(DataStreamRegistry registry, IEnumerable<IDataStreamLifetime> items)
        {
            if (registry == null)
            {
                throw new ArgumentNullException(nameof(registry));
            }

            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            _registry = registry;
            _items = items.ToArray();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            for (var i = 0; i < _items.Length; i++)
            {
                var item = _items[i];
                item.Start(_registry);
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            for (var i = _items.Length - 1; i >= 0; i--)
            {
                var item = _items[i];
                item.Start(_registry);
            }

            return Task.CompletedTask;
        }
    }

}