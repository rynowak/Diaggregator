// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Diaggregator.DataStreams
{
    public abstract class DataStream : IObservable<object>
    {
        public abstract Type DataType { get; }

        public abstract string Name { get; }

        public abstract IDisposable Subscribe(IObserver<object> observer);
    }
}