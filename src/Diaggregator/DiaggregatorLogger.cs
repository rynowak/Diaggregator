// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.Logging;

namespace Diaggregator
{
    public class DiaggregatorLogger : ILogger
    {
        private string _categoryName;
        private readonly BroadcastBlock<string> _sourceBlock;

        public DiaggregatorLogger(string categoryName)
        {
            _categoryName = categoryName;

            _sourceBlock = new BroadcastBlock<string>(s => s);
        }

        public ISourceBlock<string> SourceBlock => _sourceBlock;

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return false;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            _sourceBlock.Post(
                $"{logLevel.ToString().ToUpperInvariant()}: {_categoryName}[{eventId.Id}{Environment.NewLine}]" + 
                $"{formatter(state, exception)}" + Environment.NewLine);
        }
    }
}