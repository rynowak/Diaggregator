// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using GraphQL.Language.AST;
using GraphQL.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Diaggregator.GraphQL
{
    internal class JsonScalarGraphType : ScalarGraphType
    {
        public override object ParseLiteral(IValue value)
        {
            throw new System.NotImplementedException();
        }

        public override object ParseValue(object value)
        {
            return JsonConvert.DeserializeObject<JObject>(value.ToString());
        }

        public override object Serialize(object value)
        {
            return JsonConvert.SerializeObject(value);
        }
    }
}