// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Diaggregator.Endpoints;
using Microsoft.AspNetCore.Dispatcher;

namespace Diaggregator
{
    public class DiaggregatorDataSource : DefaultDispatcherDataSource
    {
        private readonly DiaggregatorItem[] _items;

        public DiaggregatorDataSource(IEnumerable<DiaggregatorItem> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            _items = items.OrderBy(i => i.Name).Where(i => i.GetType() != typeof(IndexEndpointHandler)).ToArray();

            var index = items.OfType<IndexEndpointHandler>().Single();
            Endpoints.Add(new HttpEndpoint("/diag", index.Invoke, index.DisplayName, Array.Empty<object>()));

            for (var i = 0; i < _items.Length; i++)
            {
                var item = _items[i];
                Endpoints.Add(new HttpEndpoint(
                    "/diag/" + (item.Template ?? item.Name),
                    item.Invoke,
                    item.DisplayName,
                    item.GetType().GetCustomAttributes(inherit: true)));
            }
        }
    }
}