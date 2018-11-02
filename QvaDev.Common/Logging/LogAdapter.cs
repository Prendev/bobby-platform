using System;
using Common.Logging;
using Common.Logging.Factory;

namespace QvaDev.Common.Logging
{
	public class LogAdapter : AbstractLogger
	{
		private readonly ILog _log;

		public LogAdapter(ILog log, bool isTraceEnabled = false)
		{
			IsTraceEnabled = isTraceEnabled;
			_log = log;
		}

		protected override void WriteInternal(LogLevel level, object message, Exception exception)
		{
			switch (level)
			{
				case LogLevel.Trace:
					if (IsTraceEnabled) _log.Trace(message, exception);
					break;
				case LogLevel.Debug:
					if (IsDebugEnabled) _log.Debug(message, exception);
					break;
				case LogLevel.Info:
					if (IsInfoEnabled) _log.Info(message, exception);
					break;
				case LogLevel.Warn:
					if (IsWarnEnabled) _log.Warn(message, exception);
					break;
				case LogLevel.Error:
					if (IsErrorEnabled) _log.Error(message, exception);
					break;
				case LogLevel.Fatal:
					if (IsFatalEnabled) _log.Fatal(message, exception);
					break;
			}
		}

		public override bool IsTraceEnabled { get; }
		public override bool IsDebugEnabled { get; } = true;
		public override bool IsErrorEnabled { get; } = true;
		public override bool IsFatalEnabled { get; } = true;
		public override bool IsInfoEnabled { get; } = true;
		public override bool IsWarnEnabled { get; } = true;
	}
}
