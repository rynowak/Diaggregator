// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks.Dataflow;

namespace Diaggregator.DataStreams
{
    internal class DefaultDataStreamRegistry : DataStreamRegistry
    {
        private readonly TransformBlock<(string, object), DataSnapshot> _block;
        private readonly IObservable<DataSnapshot> _observable;
        private readonly Dictionary<string, StreamEntry> _streams;
        private readonly object _lock;

        private DataSnapshot _values;

        public DefaultDataStreamRegistry()
        {
            _streams = new Dictionary<string, StreamEntry>();
            _lock = new object();

            _values = new DataSnapshot(_streams.Values.Select(s => s.Stream).ToArray(), 0, new Dictionary<string, object>());

            var transform = (Func<(string, object), DataSnapshot>)OnUpdate;
            _block = new TransformBlock<(string, object), DataSnapshot>(transform, new ExecutionDataflowBlockOptions()
            {
                MaxDegreeOfParallelism = 1,
            });
            _observable = DataflowBlock.AsObservable(_block);
        }

        public override DataSnapshot CurrentValues => _values;

        public override void Register(DataStream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            lock (_lock)
            {
                if (_streams.ContainsKey(stream.Name))
                {
                    throw new ArgumentException($"Duplicate key: {stream.Name}");
                }

                var disposables = new List<IDisposable>();

                var block = new TransformBlock<object, (string, object)>(data => (stream.Name, data));
                disposables.Add(block.LinkTo(_block));
                disposables.Add(stream.Subscribe(DataflowBlock.AsObserver(block)));

                var entry = new StreamEntry(stream, block, disposables.ToArray());
                _streams.Add(stream.Name, entry);
            }
        }

        public override IDisposable Subscribe(IObserver<DataSnapshot> observer)
        {
            if (observer == null)
            {
                throw new ArgumentNullException(nameof(observer));
            }

            return _observable.Subscribe(observer);
        }

        public override IDisposable Subscribe(IEnumerable<string> names, IObserver<DataSnapshot> observer)
        {
            throw new NotImplementedException();
        }

        private DataSnapshot OnUpdate((string name, object data) obj)
        {
            // This is super-not-threadsafe #YOLO
            if (_streams.Count != _values.Streams.Count)
            {
                _values = _values.WithStreams(_streams.Values.Select(s => s.Stream).ToArray());
            }

            _values = _values.WithUpdate(obj.name, obj.data);
            return _values;
        }

        private class StreamEntry
        {
            public readonly TransformBlock<object, (string, object)> Block;
            public readonly IDisposable[] Disposables;
            public readonly DataStream Stream;

            public StreamEntry(DataStream stream, TransformBlock<object, (string, object)> block, IDisposable[] disposables)
            {
                Stream = stream;
                Block = block;
                Disposables = disposables;
            }

        }
    }
}