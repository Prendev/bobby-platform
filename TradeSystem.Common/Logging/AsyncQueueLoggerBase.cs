using System;
using System.Threading;
using Common.Logging;
using Common.Logging.Factory;
using TradeSystem.Collections;

namespace TradeSystem.Common.Logging
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
				TimeStamp = HiResDatetime.UtcNow;
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
		private static readonly FastBlockingCollection<LogEntry> LogQueue =
			new FastBlockingCollection<LogEntry>();

		static AsyncQueueLoggerBase()
		{
			var logThread = new Thread(LoggingLoop) { Name = "Logging", IsBackground = true, Priority = ThreadPriority.BelowNormal};
			logThread.Start();
		}

		private static void LoggingLoop()
		{
			var counter = 0;
			while (true)
			{
				var entry = LogQueue.Take();
				var lazyMessage = new LazyMessage(() =>
					$"{entry.TimeStamp:yyyy-MM-dd HH:mm:ss.ffff} [{entry.Thread.Name ?? entry.Thread.ManagedThreadId.ToString()}] {entry.Message}");
				entry.Logger.Log(entry.Level, lazyMessage, entry.Exception);
				if (counter++ < 10) continue;
				counter = 0;
				Thread.Sleep(0);
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
