// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Diaggregator
{
    public class DescriptionMetadataAttribute : Attribute, IDescriptionMetadata
    {
        public DescriptionMetadataAttribute(string description)
        {
            Description = description;
        }
        
        public string Description { get; }
    }
}