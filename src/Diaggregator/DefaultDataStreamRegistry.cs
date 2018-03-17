// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks.Dataflow;

namespace Diaggregator
{
    internal class DefaultDataStreamRegistry : DataStreamRegistry
    {
        private readonly TransformBlock<(string, object), IReadOnlyDictionary<string, object>> _block;
        private readonly IObservable<IReadOnlyDictionary<string, object>> _observable;
        private readonly Dictionary<string, StreamEntry> _streams;
        private readonly object _lock;

        private Dictionary<string, object> _values;

        public DefaultDataStreamRegistry()
        {
            _streams = new Dictionary<string, StreamEntry>();
            _lock = new object();

            _values = new Dictionary<string, object>();

            var transform = (Func<(string, object), IReadOnlyDictionary<string, object>>)OnUpdate;
            _block = new TransformBlock<(string, object), IReadOnlyDictionary<string, object>>(transform, new ExecutionDataflowBlockOptions()
            {
                MaxDegreeOfParallelism = 1,
            });
            _observable = DataflowBlock.AsObservable(_block);
        }

        public override IReadOnlyDictionary<string, object> CurrentValues => _values;

        public override void Register(string name, IObservable<object> observable)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (observable == null)
            {
                throw new ArgumentNullException(nameof(observable));
            }

            lock (_lock)
            {
                if (_streams.ContainsKey(name))
                {
                    throw new ArgumentException($"Duplicate key: {name}");
                }

                var disposables = new List<IDisposable>();

                var block = new TransformBlock<object, (string, object)>(data => (name, data));
                disposables.Add(block.LinkTo(_block));
                disposables.Add(observable.Subscribe(DataflowBlock.AsObserver(block)));

                var entry = new StreamEntry(observable, block, disposables.ToArray());
                _streams.Add(name, entry);
            }
        }

        public override IDisposable Subscribe(IObserver<IReadOnlyDictionary<string, object>> observer)
        {
            if (observer == null)
            {
                throw new ArgumentNullException(nameof(observer));
            }

            return _observable.Subscribe(observer);
        }

        public override IDisposable Subscribe(IEnumerable<string> names, IObserver<IReadOnlyDictionary<string, object>> observer)
        {
            throw new NotImplementedException();
        }

        private IReadOnlyDictionary<string, object> OnUpdate((string name, object data) obj)
        {
            _values = new Dictionary<string, object>(_values);
            _values[obj.name] = obj.data;
            return _values;
        }

        private class StreamEntry
        {
            public readonly TransformBlock<object, (string, object)> Block;
            public readonly IDisposable[] Disposables;
            private readonly IObservable<object> Observable;

            public StreamEntry(IObservable<object> observable, TransformBlock<object, (string, object)> block, IDisposable[] disposables)
            {
                Observable = observable;
                Block = block;
                Disposables = disposables;
            }

        }
    }
}