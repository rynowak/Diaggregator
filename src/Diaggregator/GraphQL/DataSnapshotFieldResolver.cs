// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Diaggregator.DataStreams;
using GraphQL.Resolvers;
using GraphQL.Types;

namespace Diaggregator.GraphQL
{
    internal class DataSnapshotFieldResolver<T> : IFieldResolver<T>
    {
        public T Resolve(ResolveFieldContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var snapshot = (DataSnapshot)context.Source;
            snapshot.TryGetValue(context.FieldName, out var value);
            return (T)value;
        }

        object IFieldResolver.Resolve(ResolveFieldContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return Resolve(context);
        }
    }
}