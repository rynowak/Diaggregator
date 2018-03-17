// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Diaggregator
{
    public abstract class DataStreamRegistry : IObservable<IReadOnlyDictionary<string, object>>
    {
        public abstract IReadOnlyDictionary<string, object> CurrentValues { get; }

        public abstract void Register(string name, IObservable<object> observable);

        public abstract IDisposable Subscribe(IObserver<IReadOnlyDictionary<string, object>> observer);

        public abstract IDisposable Subscribe(IEnumerable<string> names, IObserver<IReadOnlyDictionary<string, object>> observer);
    }
}