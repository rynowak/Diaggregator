// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace Diaggregator
{
    internal class ConfigurationDataStream : IDataStreamLifetime
    {
        private readonly IConfiguration _configuration;
        private BroadcastBlock<IReadOnlyDictionary<string, object>> _block;
        private IDisposable _subscription;

        public ConfigurationDataStream(IConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            _configuration = configuration;
        }

        public void Start(DataStreamRegistry registry)
        {
            if (registry == null)
            {
                throw new ArgumentNullException(nameof(registry));
            }

            _block = new BroadcastBlock<IReadOnlyDictionary<string, object>>(i => i);
            registry.Register("configuration", _block.AsObservable());

            _block.Post(GetValues());

            _subscription = ChangeToken.OnChange(() => _configuration.GetReloadToken(), () =>
            {
                _block.Post(GetValues());
            });
        }

        public void Stop(DataStreamRegistry registry)
        {
            if (registry == null)
            {
                throw new ArgumentNullException(nameof(registry));
            }

            _subscription.Dispose();
            _subscription = null;

            _block.Complete();
            _block = null;
        }

        private IReadOnlyDictionary<string, object> GetValues()
        {
            return _configuration.GetChildren().ToDictionary(s => s.Key, Convert);
        }

        private static object Convert(IConfigurationSection section)
        {
            return (object)section.Value ?? section.GetChildren().ToDictionary(c => c.Key, Convert);
        }
    }
}