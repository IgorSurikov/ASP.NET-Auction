using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Auction.Logger
{
	public class FileLogger : ILogger
	{
		private readonly string _errorFilePath;
		private readonly string _logsFilePath;

		public FileLogger()
		{
			_errorFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "logging", "errors");
			_logsFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "logging", "logs");
		}

		public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
			Func<TState, Exception, string> formatter)
		{
			if (!IsEnabled(logLevel)) return;
			File.AppendAllText(_logsFilePath, DateTime.Now + "-" + formatter(state, exception) + Environment.NewLine);
			Console.WriteLine($"{DateTime.Now}-{formatter(state, exception)}");
			if (logLevel >= LogLevel.Error)
			{
				File.AppendAllText(_errorFilePath,   DateTime.Now +"-"+ formatter(state, exception) + Environment.NewLine);
			}
		}

		public bool IsEnabled(LogLevel logLevel)
		{
			return logLevel >= LogLevel.Debug;
		}

		public IDisposable BeginScope<TState>(TState state)
		{
			return null;
		}
	}
}