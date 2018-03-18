// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Reactive.Linq;
using System.Threading.Tasks.Dataflow;

namespace Diaggregator.DataStreams
{
    public class DefaultDataStream<T> : DataStream
    {
        private readonly IObservable<T> _observable;

        public DefaultDataStream(string name, Type dataType, IObservable<T> observable)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (observable == null)
            {
                throw new ArgumentNullException(nameof(observable));
            }

            Name = name;
            DataType = dataType;
            _observable = observable;
        }

        public override Type DataType { get; }

        public override string Name { get; }

        public override IDisposable Subscribe(IObserver<object> observer)
        {
            if (observer == null)
            {
                throw new ArgumentNullException(nameof(observer));
            }
            
            return _observable.Select(v => (object)v).Subscribe(observer);
        }
    }
}