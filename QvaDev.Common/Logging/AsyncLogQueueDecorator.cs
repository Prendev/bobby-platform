using System;
using System.Collections.Concurrent;
using System.Threading;
using log4net.Core;

namespace QvaDev.Common.Logging
{
	public class AsyncLogQueueDecorator : ICustomLog
	{
		private class LogEntry
		{
			public LogEntry(object message, Exception exception, Action<object, Exception> action)
			{
				Message = message;
				Exception = exception;
				Action = action;
				TimeStamp = DateTime.UtcNow;
				Thread = Thread.CurrentThread;
			}

			public object Message { get; }
			public Exception Exception { get; }
			public Action<object, Exception> Action { get; }
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

		private readonly log4net.ILog _log;

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
				var lazyMessage = new LazyMessage(() =>
					$"{entry.TimeStamp:yyyy-MM-dd HH:mm:ss.fff} [{entry.Thread.Name ?? entry.Thread.ManagedThreadId.ToString()}] {entry.Message}");
				entry.Action(lazyMessage, entry.Exception);
			}
		}

		public AsyncLogQueueDecorator(log4net.ILog log)
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

		public void Trace(object message) => Trace(message, null);
		public void Trace(object message, Exception exception) =>
			LogQueue.Add(new LogEntry(message, exception, TraceInner));

		public void Verbose(object message) => Verbose(message, null);
		public void Verbose(object message, Exception exception) =>
			LogQueue.Add(new LogEntry(message, exception, VerboseInner));

		private void TraceInner(object message, Exception exception) => Logger.Log(GetType(), Level.Trace, message, exception);
		private void VerboseInner(object message, Exception exception) => Logger.Log(GetType(), Level.Verbose, message, exception);
	}
}
