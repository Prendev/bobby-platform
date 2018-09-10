using System;
using Common.Logging;
using Common.Logging.Factory;

namespace QvaDev.Common.Logging
{
	public class LogAdapter : AbstractLogger
	{
		private readonly log4net.ILog _log4NetLog;

		public LogAdapter(log4net.ILog log4NetLog)
		{
			_log4NetLog = log4NetLog;
		}

		protected override void WriteInternal(LogLevel level, object message, Exception exception)
		{
			switch (level)
			{
				case LogLevel.Debug:
					_log4NetLog.Debug(message, exception);
					break;
				case LogLevel.Info:
					_log4NetLog.Info(message, exception);
					break;
				case LogLevel.Warn:
					_log4NetLog.Warn(message, exception);
					break;
				case LogLevel.Error:
					_log4NetLog.Error(message, exception);
					break;
				case LogLevel.Fatal:
					_log4NetLog.Fatal(message, exception);
					break;
				default:
					_log4NetLog.Debug(message, exception);
					break;
			}
		}

		public override bool IsTraceEnabled { get; } = false;
		public override bool IsDebugEnabled { get; } = true;
		public override bool IsErrorEnabled { get; } = true;
		public override bool IsFatalEnabled { get; } = true;
		public override bool IsInfoEnabled { get; } = true;
		public override bool IsWarnEnabled { get; } = true;
	}
}
