// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.AspNetCore.Dispatcher;
using Microsoft.AspNetCore.Http;

namespace Diaggregator
{
    [DescriptionMetadata("Streams log messages")]
    public class LogStreamEndpointHandler : DiaggregatorItem
    {
        private readonly DiaggregatorLoggerProvider _loggerProvider;

        public override string DisplayName => "Streaming Logs";

        public override string Name => "logstream";

        public override string Template => "log/{category}";

        public LogStreamEndpointHandler(DiaggregatorLoggerProvider loggerProvider)
        {
            if (loggerProvider == null)
            {
                throw new ArgumentNullException(nameof(loggerProvider));
            }

            _loggerProvider = loggerProvider;
        }
        
        public async override Task Invoke(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var feature = context.Features.Get<IDispatcherFeature>();
            var categoryName = (string)feature.Values["category"];

            var loggers = GetLoggers(categoryName);
            if (loggers.Length == 0)
            {
                context.Response.StatusCode = 400;
                return;
            }

            context.Response.ContentType = "text/plain";

            var faulted = new CancellationTokenSource();
            var cancelled = CancellationTokenSource.CreateLinkedTokenSource(faulted.Token, context.RequestAborted);

            var actionBlock = new ActionBlock<string>(async (log) => 
            {
                try
                
                {
                    await SendLog(context, log, cancelled.Token);
                }
                catch (Exception)
                {
                    faulted.Cancel();
                }
            }, new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = 1, });

            cancelled.Token.Register(() => 
            {
                actionBlock.Complete();
            });

            for (var i = 0; i < loggers.Length; i++)
            {
                loggers[i].SourceBlock.LinkTo(actionBlock);
            }

            await actionBlock.Completion;
        }

        private async Task SendLog(HttpContext httpContext, string log, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            await httpContext.Response.WriteAsync(log, cancellationToken);
            await httpContext.Response.Body.FlushAsync(cancellationToken);
        }

        private DiaggregatorLogger[] GetLoggers(string categoryName)
        {
            DiaggregatorLogger[] loggers;
            var categoryNames = _loggerProvider.GetCategoryNames();
            if (string.IsNullOrEmpty(categoryName))
            {
                loggers = new DiaggregatorLogger[categoryNames.Count];
                for (var i = 0; i < categoryNames.Count; i++)
                {
                    loggers[i] = (DiaggregatorLogger)_loggerProvider.CreateLogger(categoryNames[i]);
                }

                return loggers;
            }


            var start = 0;
            for (; start < categoryNames.Count; start++)
            {
                if (StringComparer.Ordinal.Compare(categoryName, categoryNames[start]) != 0)
                {
                    break;
                }
            }

            var end = start + 1;
            for (; end < categoryNames.Count; end++)
            {
                if (!categoryNames[end].StartsWith(categoryName) || 
                    categoryNames[end].Length <= categoryName.Length &&
                    categoryNames[end][categoryName.Length] != '.')
                {
                    break;
                }
            }

            loggers = new DiaggregatorLogger[end - start];
            for (var i = start; i < end; i++)
            {
                loggers[i - start] = (DiaggregatorLogger)_loggerProvider.CreateLogger(categoryNames[i]);
            }

            return loggers;
        }
    }
}