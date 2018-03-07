// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Diaggregator
{
    public abstract class DiaggregatorItem
    {
        public string DisplayName { get; }

        public string Description { get; }

        public string ShortName { get; } 
    }
}