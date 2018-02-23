using System;
using Microsoft.Extensions.Logging;

namespace Featureflow.Client
{
    internal static class FeatureflowLogger
	{
		internal static ILoggerFactory LoggerFactory = new LoggerFactory();

		internal static ILogger CreateLogger<T>()
		{
			return LoggerFactory.CreateLogger<T>();
		}

		internal static ILogger CreateLogger(string categoryName)
		{
			return LoggerFactory.CreateLogger(categoryName);
		}
	}
}
