using System;
using System.Collections.Concurrent;
using System.Threading;
using log4net;
using log4net.Core;

namespace QvaDev.Common.Logging
{
	public class AsyncLogQueueDecorator : ILog
	{
		private class LogEntry
		{
			public LogEntry(object message, Exception exception, Action<string, Exception> action)
			{
				Message = message;
				Exception = exception;
				Action = action;
				TimeStamp = DateTime.UtcNow;
				Thread = Thread.CurrentThread;
			}

			public object Message { get; }
			public Exception Exception { get; }
			public Action<string, Exception> Action { get; }
			public DateTime TimeStamp { get; }
			public Thread Thread { get; }
		}

		private readonly ILog _log;

		public ILogger Logger => _log.Logger;

		private static readonly BlockingCollection<LogEntry> LogQueue =
			new BlockingCollection<LogEntry>();

		public bool IsDebugEnabled => _log.IsDebugEnabled;
		public bool IsInfoEnabled => _log.IsInfoEnabled;
		public bool IsWarnEnabled => _log.IsWarnEnabled;
		public bool IsErrorEnabled => _log.IsErrorEnabled;
		public bool IsFatalEnabled => _log.IsFatalEnabled;

		static AsyncLogQueueDecorator()
		{
			var logThread = new Thread(LoggingLoop) { Name = "Logging", IsBackground = true };
			logThread.Start();
		}

		private static void LoggingLoop()
		{
			while (true)
			{
				var entry = LogQueue.Take();
				var message =
					$"{entry.TimeStamp:yyyy-MM-dd HH:mm:ss.fff} [{entry.Thread.Name ?? entry.Thread.ManagedThreadId.ToString()}] {entry.Message}";
				entry.Action(message, entry.Exception);
			}
		}

		public AsyncLogQueueDecorator(ILog log)
		{
			_log = log;
		}

		public void Debug(object message) => Debug(message, null);
		public void Debug(object message, Exception exception) =>
			LogQueue.Add(new LogEntry(message, exception, _log.Debug));

		public void DebugFormat(string format, params object[] args) => throw new NotImplementedException();
		public void DebugFormat(string format, object arg0) => throw new NotImplementedException();
		public void DebugFormat(string format, object arg0, object arg1) => throw new NotImplementedException();
		public void DebugFormat(string format, object arg0, object arg1, object arg2) => throw new NotImplementedException();
		public void DebugFormat(IFormatProvider provider, string format, params object[] args) => throw new NotImplementedException();

		public void Info(object message) => Info(message, null);
		public void Info(object message, Exception exception) =>
			LogQueue.Add(new LogEntry(message, exception, _log.Info));

		public void InfoFormat(string format, params object[] args) => throw new NotImplementedException();
		public void InfoFormat(string format, object arg0) => throw new NotImplementedException();
		public void InfoFormat(string format, object arg0, object arg1) => throw new NotImplementedException();
		public void InfoFormat(string format, object arg0, object arg1, object arg2) => throw new NotImplementedException();
		public void InfoFormat(IFormatProvider provider, string format, params object[] args) => throw new NotImplementedException();

		public void Warn(object message) => Warn(message, null);
		public void Warn(object message, Exception exception) =>
			LogQueue.Add(new LogEntry(message, exception, _log.Warn));

		public void WarnFormat(string format, params object[] args) => throw new NotImplementedException();
		public void WarnFormat(string format, object arg0) => throw new NotImplementedException();
		public void WarnFormat(string format, object arg0, object arg1) => throw new NotImplementedException();
		public void WarnFormat(string format, object arg0, object arg1, object arg2) => throw new NotImplementedException();
		public void WarnFormat(IFormatProvider provider, string format, params object[] args) => throw new NotImplementedException();

		public void Error(object message) => Error(message, null);
		public void Error(object message, Exception exception) =>
			LogQueue.Add(new LogEntry(message, exception, _log.Error));

		public void ErrorFormat(string format, params object[] args) => throw new NotImplementedException();
		public void ErrorFormat(string format, object arg0) => throw new NotImplementedException();
		public void ErrorFormat(string format, object arg0, object arg1) => throw new NotImplementedException();
		public void ErrorFormat(string format, object arg0, object arg1, object arg2) => throw new NotImplementedException();
		public void ErrorFormat(IFormatProvider provider, string format, params object[] args) => throw new NotImplementedException();

		public void Fatal(object message) => Fatal(message, null);
		public void Fatal(object message, Exception exception) =>
			LogQueue.Add(new LogEntry(message, exception, _log.Fatal));

		public void FatalFormat(string format, params object[] args) => throw new NotImplementedException();
		public void FatalFormat(string format, object arg0) => throw new NotImplementedException();
		public void FatalFormat(string format, object arg0, object arg1) => throw new NotImplementedException();
		public void FatalFormat(string format, object arg0, object arg1, object arg2) => throw new NotImplementedException();
		public void FatalFormat(IFormatProvider provider, string format, params object[] args) => throw new NotImplementedException();
	}
}
