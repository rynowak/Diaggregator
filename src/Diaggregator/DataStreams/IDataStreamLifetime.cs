// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Diaggregator.DataStreams
{
    public interface IDataStreamLifetime
    {
        void Start(DataStreamRegistry registry);

        void Stop(DataStreamRegistry registry);
    }
}