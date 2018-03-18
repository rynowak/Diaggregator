// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Diaggregator.DataStreams;
using GraphQL.Types;

namespace Diaggregator.GraphQL
{
    internal class DataSnapshotQuery : ObjectGraphType<object>
    {
        public DataSnapshotQuery(DataSnapshotSchema parent, IReadOnlyList<DataStream> streams)
        {
            if (parent == null)
            {
                throw new ArgumentNullException(nameof(parent));
            }

            Parent = parent;

            for (var i = 0; i < streams.Count; i++)
            {
                var stream = streams[i];

                AddField(new FieldType()
                {
                    Name = stream.Name,
                    Type = stream.DataType,
                    ResolvedType = Parent.ResolveType(stream.DataType),
                    Resolver = new DataSnapshotFieldResolver<object>(),
                });
            }
        }

        public DataSnapshotSchema Parent { get; }
    }
}