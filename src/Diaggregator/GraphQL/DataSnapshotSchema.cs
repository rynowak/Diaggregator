// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Diaggregator.DataStreams;
using GraphQL.Resolvers;
using GraphQL.Types;

namespace Diaggregator.GraphQL
{
    internal class DataSnapshotSchema : Schema
    {
        private readonly IReadOnlyList<DataStream> _streams;

        public DataSnapshotSchema(IReadOnlyList<DataStream> streams)
        {
            if (streams == null)
            {
                throw new System.ArgumentNullException(nameof(streams));
            }

            _streams = streams;

            ResolveType = OnResolvingType;

            Query = new DataSnapshotQuery(this, _streams);
        }

        private static IGraphType OnResolvingType(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (type == typeof(object))
            {
                return new JsonScalarGraphType();
            }

            if (type == typeof(string))
            {
                return new StringGraphType();
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                var inner = type.GetGenericArguments()[0];
                var list = new ListGraphType(OnResolvingType(inner));
                return list;
            }

            if (type.IsArray && type.GetElementType() != null)
            {
                var inner = type.GetElementType();
                var list = new ListGraphType(OnResolvingType(inner));
                return list;
            }

            var graphType = typeof(GoodObjectGraphType<>).MakeGenericType(new[]{ type });
            var complex = (IComplexGraphType)Activator.CreateInstance(graphType, new[]{ type });
            complex.Name = type.Name;

            return complex;
        }

        private class GoodObjectGraphType<T> : ObjectGraphType<T>
        {
            public GoodObjectGraphType(Type type)
            {
                foreach (var property in type.GetProperties())
                {
                    AddField(new FieldType()
                    {
                        Name = property.Name,
                        Type = property.PropertyType,
                        ResolvedType = OnResolvingType(property.PropertyType),
                        Resolver = new FuncFieldResolver<object>(c => property.GetValue(c.Source)),
                    });
                }
            }
        } 
    }
}