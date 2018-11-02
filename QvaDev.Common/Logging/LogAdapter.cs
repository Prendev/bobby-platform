using System;
using Common.Logging;
using Common.Logging.Factory;

namespace QvaDev.Common.Logging
{
	public class LogAdapter : AbstractLogger
	{
		private readonly ILog _log;

		public LogAdapter(ILog log)
		{
			_log = log;
		}

		protected override void WriteInternal(LogLevel level, object message, Exception exception)
		{
			switch (level)
			{
				case LogLevel.Trace:
					_log.Trace(message, exception);
					break;
				case LogLevel.Debug:
					_log.Debug(message, exception);
					break;
				case LogLevel.Info:
					_log.Info(message, exception);
					break;
				case LogLevel.Warn:
					_log.Warn(message, exception);
					break;
				case LogLevel.Error:
					_log.Error(message, exception);
					break;
				case LogLevel.Fatal:
					_log.Fatal(message, exception);
					break;
			}
		}

		public override bool IsTraceEnabled { get; } = true;
		public override bool IsDebugEnabled { get; } = true;
		public override bool IsErrorEnabled { get; } = true;
		public override bool IsFatalEnabled { get; } = true;
		public override bool IsInfoEnabled { get; } = true;
		public override bool IsWarnEnabled { get; } = true;
	}
}
