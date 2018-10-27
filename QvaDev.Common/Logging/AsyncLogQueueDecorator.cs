using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Windows.Forms;
using log4net;
using log4net.Core;

namespace QvaDev.Common.Logging
{
	public class AsyncLogQueueDecorator : ILog
	{
		private readonly ILog _log;

		public ILogger Logger => _log.Logger;

		private static readonly ConcurrentQueue<MethodInvoker> LogQueue =
			new ConcurrentQueue<MethodInvoker>();

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

		public AsyncLogQueueDecorator(ILog log)
		{
			_log = log;
		}

		public void Debug(object message) =>
			LogQueue.Enqueue(() => _log.Debug(message));
		public void Debug(object message, Exception exception) =>
			LogQueue.Enqueue(() => _log.Debug(message, exception));
		public void DebugFormat(string format, params object[] args) =>
			LogQueue.Enqueue(() => _log.DebugFormat(format, args));
		public void DebugFormat(string format, object arg0) =>
			LogQueue.Enqueue(() => _log.DebugFormat(format, arg0));
		public void DebugFormat(string format, object arg0, object arg1) =>
			LogQueue.Enqueue(() => _log.DebugFormat(format, arg0, arg1));
		public void DebugFormat(string format, object arg0, object arg1, object arg2) =>
			LogQueue.Enqueue(() => _log.DebugFormat(format, arg0, arg1, arg2));
		public void DebugFormat(IFormatProvider provider, string format, params object[] args) =>
			LogQueue.Enqueue(() => _log.DebugFormat(provider, format, args));
		public void Info(object message) =>
			LogQueue.Enqueue(() => _log.Info(message));
		public void Info(object message, Exception exception) =>
			LogQueue.Enqueue(() => _log.Info(message, exception));
		public void InfoFormat(string format, params object[] args) =>
			LogQueue.Enqueue(() => _log.InfoFormat(format, args));
		public void InfoFormat(string format, object arg0) =>
			LogQueue.Enqueue(() => _log.InfoFormat(format, arg0));
		public void InfoFormat(string format, object arg0, object arg1) =>
			LogQueue.Enqueue(() => _log.InfoFormat(format, arg0, arg1));
		public void InfoFormat(string format, object arg0, object arg1, object arg2) =>
			LogQueue.Enqueue(() => _log.InfoFormat(format, arg0, arg1, arg2));
		public void InfoFormat(IFormatProvider provider, string format, params object[] args) =>
			LogQueue.Enqueue(() => _log.InfoFormat(provider, format, args));
		public void Warn(object message) =>
			LogQueue.Enqueue(() => _log.Warn(message));
		public void Warn(object message, Exception exception) =>
			LogQueue.Enqueue(() => _log.Warn(message, exception));
		public void WarnFormat(string format, params object[] args) =>
			LogQueue.Enqueue(() => _log.WarnFormat(format, args));
		public void WarnFormat(string format, object arg0) =>
			LogQueue.Enqueue(() => _log.WarnFormat(format, arg0));
		public void WarnFormat(string format, object arg0, object arg1) =>
			LogQueue.Enqueue(() => _log.WarnFormat(format, arg0, arg1));
		public void WarnFormat(string format, object arg0, object arg1, object arg2) =>
			LogQueue.Enqueue(() => _log.WarnFormat(format, arg0, arg1, arg2));
		public void WarnFormat(IFormatProvider provider, string format, params object[] args) =>
			LogQueue.Enqueue(() => _log.WarnFormat(provider, format, args));
		public void Error(object message) =>
			LogQueue.Enqueue(() => _log.Error(message));
		public void Error(object message, Exception exception) =>
			LogQueue.Enqueue(() => _log.Error(message, exception));
		public void ErrorFormat(string format, params object[] args) =>
			LogQueue.Enqueue(() => _log.ErrorFormat(format, args));
		public void ErrorFormat(string format, object arg0) =>
			LogQueue.Enqueue(() => _log.ErrorFormat(format, arg0));
		public void ErrorFormat(string format, object arg0, object arg1) =>
			LogQueue.Enqueue(() => _log.ErrorFormat(format, arg0, arg1));
		public void ErrorFormat(string format, object arg0, object arg1, object arg2) =>
			LogQueue.Enqueue(() => _log.ErrorFormat(format, arg0, arg1, arg2));
		public void ErrorFormat(IFormatProvider provider, string format, params object[] args) =>
			LogQueue.Enqueue(() => _log.ErrorFormat(provider, format, args));
		public void Fatal(object message) =>
			LogQueue.Enqueue(() => _log.Fatal(message));
		public void Fatal(object message, Exception exception) =>
			LogQueue.Enqueue(() => _log.Fatal(message, exception));
		public void FatalFormat(string format, params object[] args) =>
			LogQueue.Enqueue(() => _log.FatalFormat(format, args));
		public void FatalFormat(string format, object arg0) =>
			LogQueue.Enqueue(() => _log.FatalFormat(format, arg0));
		public void FatalFormat(string format, object arg0, object arg1) =>
			LogQueue.Enqueue(() => _log.FatalFormat(format, arg0, arg1));
		public void FatalFormat(string format, object arg0, object arg1, object arg2) =>
			LogQueue.Enqueue(() => _log.FatalFormat(format, arg0, arg1, arg2));
		public void FatalFormat(IFormatProvider provider, string format, params object[] args) =>
			LogQueue.Enqueue(() => _log.FatalFormat(provider, format, args));

		private static void LoggingLoop()
		{
			while (true)
			{
				while (LogQueue.TryDequeue(out var m)) m();
				Thread.Sleep(10);
			}
		}
	}
}
