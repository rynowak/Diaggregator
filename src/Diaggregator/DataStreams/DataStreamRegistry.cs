// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Diaggregator.DataStreams
{
    public abstract class DataStreamRegistry : IObservable<DataSnapshot>
    {
        public abstract DataSnapshot CurrentValues { get; }

        public void Register<T>(string name, IObservable<T> observable)
        {
            Register(new DefaultDataStream<T>(name, typeof(T), observable));
        }

        public void Register(string name, Type dataType, IObservable<object> observable)
        {
            Register(new DefaultDataStream<object>(name, dataType, observable));
        }

        public abstract void Register(DataStream stream);

        public abstract IDisposable Subscribe(IObserver<DataSnapshot> observer);

        public abstract IDisposable Subscribe(IEnumerable<string> names, IObserver<DataSnapshot> observer);
    }
}