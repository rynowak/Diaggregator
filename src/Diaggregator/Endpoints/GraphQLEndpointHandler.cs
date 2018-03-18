// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Diaggregator.DataStreams;
using Diaggregator.GraphQL;
using GraphQL;
using GraphQL.Http;
using GraphQL.Resolvers;
using GraphQL.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Dispatcher;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Diaggregator.Endpoints
{
    [DescriptionMetadata("Lists all routeable endpoints in the application")]
    public class GraphQLEndpointHandler : DiaggregatorItem
    {
        private readonly ILogger<GraphQLEndpointHandler> _logger;
        private readonly DataStreamRegistry _registry;
        private readonly IDocumentExecuter _executor;
        private readonly IDocumentWriter _writer;

        public GraphQLEndpointHandler(
            ILogger<GraphQLEndpointHandler> logger,
            DataStreamRegistry registry, 
            IDocumentExecuter executor, 
            IDocumentWriter writer)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            if (registry == null)
            {
                throw new ArgumentNullException(nameof(registry));
            }

            if (executor == null)
            {
                throw new ArgumentNullException(nameof(executor));
            }

            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            _logger = logger;
            _registry = registry;
            _executor = executor;
            _writer = writer;
        }

        public override string DisplayName => "GraphQL";

        public override string Name => "graphql";

        public override string Template => "/graphql";

        public async override Task Invoke(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            GraphQLQuery query = null;
            if (context.Request.Method == HttpMethods.Get)
            {
                query = new GraphQLQuery();
                query.Query = context.Request.Query["query"];
                query.OperationName = context.Request.Query["operationName"];

                if (context.Request.Query["variables"].Count == 1)
                {
                    query.Variables = JsonConvert.DeserializeObject<JObject>(context.Request.Query["variables"]);
                }
            }
            else if (context.Request.Method == HttpMethods.Post && context.Request.ContentType == "application/json")
            {
                using (var reader = new HttpRequestStreamReader(context.Request.Body, Encoding.UTF8))
                {
                    var text = await reader.ReadToEndAsync();
                    query = JsonConvert.DeserializeObject<GraphQLQuery>(text);
                }
            }

            if (string.IsNullOrEmpty(query?.Query))
            {
                context.Response.StatusCode = 400;
                return;
            }

            var root = _registry.CurrentValues;

            var schema = new DataSnapshotSchema(root.Streams);

            var inputs = query.Variables == null ? null : new Inputs(query.Variables.AsDictionary());
            var options = new ExecutionOptions()
            {
                OperationName = query.OperationName,
                Query = query.Query,
                Inputs = inputs,

                Root = root,
                Schema = schema,    
            };

            var result = await _executor.ExecuteAsync(options);
            if (result.Errors != null && result.Errors.Count > 0)
            {
                foreach (var error in result.Errors)
                {
                    _logger.LogError(error.InnerException, error.ToString());
                }
            }

            var json = _writer.Write(result);

            context.Response.StatusCode = 200;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(json, Encoding.UTF8);
        }

        private class GraphQLQuery
        {
            public string OperationName { get; set; }
            public string Query { get; set; }
            public JObject Variables { get; set; }
        }
    }
}