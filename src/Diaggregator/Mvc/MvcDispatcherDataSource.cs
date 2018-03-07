// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using Microsoft.AspNetCore.Dispatcher;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Diaggregator.Mvc
{
    public class MvcDispatcherDataSource : DefaultDispatcherDataSource
    {
        private readonly IActionInvokerFactory _invokerFactory;

        public MvcDispatcherDataSource(
            IActionDescriptorCollectionProvider actions,
            IActionInvokerFactory invokerFactory)
        {
            if (actions == null)
            {
                throw new ArgumentNullException(nameof(actions));
            }

            if (invokerFactory == null)
            {
                throw new ArgumentNullException(nameof(invokerFactory));
            }

            _invokerFactory = invokerFactory;

            // note: this code has haxxx. This will only work in some constrained scenarios
            foreach (var action in actions.ActionDescriptors.Items)
            {
                Endpoints.Add(new HttpEndpoint(
                    action.AttributeRouteInfo.Template,
                    action.RouteValues,
                    action.ActionConstraints?.OfType<HttpMethodActionConstraint>().FirstOrDefault()?.HttpMethods.FirstOrDefault(),
                    async (context) =>
                    {
                        var values = context.Features.Get<IDispatcherFeature>().Values;
                        var routeData = new RouteData();
                        foreach (var kvp in values)
                        {
                            routeData.Values.Add(kvp.Key, kvp.Value);
                        }

                        var actionContext = new ActionContext(context, routeData, action);
                        var invoker = _invokerFactory.CreateInvoker(actionContext);
                        await invoker.InvokeAsync();
                    },
                    action.DisplayName,
                    action.FilterDescriptors.OrderBy(f => f, FilterDescriptorOrderComparer.Comparer).Select(f => f.Filter).ToArray()));
            }
        }
    }
}