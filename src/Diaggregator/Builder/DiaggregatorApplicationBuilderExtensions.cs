// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Diaggregator.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Builder
{
    public static class DiaggregatorApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseAuthorization(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.Use(next => async (context) =>
            {
                var services = context.RequestServices;
                var policyProvider = services.GetRequiredService<IAuthorizationPolicyProvider>();
                var policyEvaluator = services.GetRequiredService<IPolicyEvaluator>();

                var middleware = new AuthorizationMiddleware(next, policyProvider);
                await middleware.Invoke(context);
            });
        }
    }
}
