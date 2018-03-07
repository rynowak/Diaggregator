// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Diaggregator
{
    public abstract class DiaggregatorItem
    {
        public abstract string DisplayName { get; }

        public abstract string Name { get; }

        public abstract string Template { get; }

        public abstract Task Invoke(HttpContext context);
    }
}