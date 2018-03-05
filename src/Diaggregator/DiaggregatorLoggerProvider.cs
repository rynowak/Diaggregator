// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Diaggregator
{
    public class DiaggregatorLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, bool> _categoryNames;
        private readonly ConcurrentDictionary<string, DiaggregatorLogger> _loggers;

        public DiaggregatorLoggerProvider()
        {
            _categoryNames = new ConcurrentDictionary<string, bool>();
            _loggers = new ConcurrentDictionary<string, DiaggregatorLogger>();
        }

        public IReadOnlyList<string> GetCategoryNames()
        {
            return _categoryNames.Select(kvp => kvp.Key).OrderBy(n => n).ToArray();
        }

        public ILogger CreateLogger(string categoryName)
        {
            if (categoryName == null)
            {
                throw new ArgumentNullException(nameof(categoryName));
            }

            if (!_loggers.TryGetValue(categoryName, out var logger))
            {
                logger = new DiaggregatorLogger(categoryName);
                if (ReferenceEquals(_loggers.GetOrAdd(categoryName, logger), logger))
                {
                    do
                    {
                        if (!_categoryNames.TryAdd(categoryName, true))
                        {
                            break;
                        }

                        var index = categoryName.LastIndexOf('.');
                        categoryName = index == -1 ? null : categoryName.Substring(0, index);
                    } 
                    while (categoryName != null);
                }
            }

            return logger;
        }

        public void Dispose()
        {
        }
    }
}