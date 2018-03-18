// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Diaggregator.DataStreams
{
    public class ConfigurationEntry
    {
        public ConfigurationEntry(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; }

        public string Value { get; }
    }
}