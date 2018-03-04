﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.AspNetCore.Dispatcher
{
    public interface IRoutePatternEndpoint
    {
        string HttpMethod { get; }

        string Pattern { get; }

        DispatcherValueCollection Values { get; }
    }
}
