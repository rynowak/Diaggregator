// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Diaggregator
{
    public class DiaggregatorEndpointMetadata : IDiaggregatorEndpointMetadata
    {
        public string DisplayName { get; set; }

        public string Description { get; set; }

        public string ShortName { get; set; }
    }
}