﻿using System;
using Common.Logging;
using log4net.Core;
using ILog = log4net.ILog;

namespace TradeSystem.Common.Logging
{
	public class LogAdapter : AsyncQueueLoggerBase
	{
		private readonly ILog _log;

		public LogAdapter(ILog log)
		{
			_log = log;
		}

		protected override void Log(LogLevel level, object message, Exception exception)
		{
			switch (level)
			{
				case LogLevel.Trace:
					_log.Logger.Log(GetType(), Level.Trace, message, exception);
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
