using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Auction.Logger
{
	public class FileLoggerProvider : ILoggerProvider
	{
		private readonly ConcurrentDictionary<string, FileLogger> _loggers = new ConcurrentDictionary<string, FileLogger>();
		public void Dispose()
		{
			_loggers.Clear();
		}
			
		public ILogger CreateLogger(string categoryName)
		{
			return _loggers.GetOrAdd(categoryName, factory => new FileLogger());
		}
	}
}
