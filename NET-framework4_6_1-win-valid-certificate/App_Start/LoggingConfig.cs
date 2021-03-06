﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Steeltoe.Extensions.Logging;

namespace NET_framework4_6_1_win_valid_certificate
{
	public static class LoggingConfig
	{
		public static ILoggerFactory LoggerFactory { get; set; }
		public static ILoggerProvider LoggerProvider { get; set; }

		public static void Configure(IConfiguration configuration)
		{
			LoggerProvider = new DynamicLoggerProvider(new ConsoleLoggerSettings().FromConfiguration(configuration));
			LoggerFactory = new LoggerFactory();
			LoggerFactory.AddProvider(LoggerProvider);
		}
	}
}