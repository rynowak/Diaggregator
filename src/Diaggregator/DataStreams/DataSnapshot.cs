// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Diaggregator.DataStreams
{
    public class DataSnapshot : IReadOnlyDictionary<string, object>
    {
        private readonly Dictionary<string, object> _values;

        internal DataSnapshot(DataStream[] streams, int version, Dictionary<string, object> values)
        {
            if (streams == null)
            {
                throw new ArgumentNullException(nameof(streams));
            }

            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            Streams = streams;
            Version = version;
            _values = values;
        }

        public object this[string key] => _values[key];

        public IEnumerable<string> Keys => ((IReadOnlyDictionary<string, object>)_values).Keys;

        public IEnumerable<object> Values => ((IReadOnlyDictionary<string, object>)_values).Values;

        public int Count => _values.Count;

        public IReadOnlyList<DataStream> Streams { get; }

        public int Version { get; }

        public DataSnapshot WithStreams(DataStream[] streams)
        {
            if (streams == null)
            {
                throw new ArgumentNullException(nameof(streams));
            }

            return new DataSnapshot(streams, Version + 1, _values);
        }

        public DataSnapshot WithUpdate(string key, object value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            var values = new Dictionary<string, object>(_values);
            values[key] = value;

            return new DataSnapshot(Streams.ToArray(), Version + 1, values);
        }

        public bool ContainsKey(string key)
        {
            return _values.ContainsKey(key);
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return ((IReadOnlyDictionary<string, object>)_values).GetEnumerator();
        }

        public bool TryGetValue(string key, out object value)
        {
            return _values.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IReadOnlyDictionary<string, object>)_values).GetEnumerator();
        }
    }
}