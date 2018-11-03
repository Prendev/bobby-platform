using System;
using System.Collections.Concurrent;
using System.Threading;
using Common.Logging;
using Common.Logging.Factory;

namespace QvaDev.Common.Logging
{
	public abstract class AsyncQueueLoggerBase : AbstractLogger
	{
		private class LogEntry
		{
			public LogEntry(AsyncQueueLoggerBase logger, LogLevel level, object message, Exception exception)
			{
				Logger = logger;
				Level = level;
				Message = message;
				Exception = exception;
				TimeStamp = DateTime.UtcNow;
				Thread = Thread.CurrentThread;
			}

			public AsyncQueueLoggerBase Logger { get; }
			public LogLevel Level { get; }
			public object Message { get; }
			public Exception Exception { get; }
			public DateTime TimeStamp { get; }
			public Thread Thread { get; }
		}

		private class LazyMessage
		{
			private readonly Func<string> _message;

			public LazyMessage(Func<string> message)
			{
				_message = message;
			}

			public override string ToString()
			{
				return _message();
			}
		}
		private static readonly BlockingCollection<LogEntry> LogQueue =
			new BlockingCollection<LogEntry>();

		static AsyncQueueLoggerBase()
		{
			var logThread = new Thread(LoggingLoop) { Name = "Logging", IsBackground = true };
			logThread.Start();
		}

		private static void LoggingLoop()
		{
			while (true)
			{
				var entry = LogQueue.Take();
				var lazyMessage = new LazyMessage(() =>
					$"{entry.TimeStamp:yyyy-MM-dd HH:mm:ss.fff} [{entry.Thread.Name ?? entry.Thread.ManagedThreadId.ToString()}] {entry.Message}");
				entry.Logger.Log(entry.Level, lazyMessage, entry.Exception);
			}
		}

		protected sealed override void WriteInternal(LogLevel level, object message, Exception exception)
		{
			LogQueue.Add(new LogEntry(this, level, message, exception));
		}

		protected virtual void Log(LogLevel level, object message, Exception exception)
		{
		}
	}
}
