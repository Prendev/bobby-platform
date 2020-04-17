//---------------------------------------------------------------------------------------------------------------------
// <copyright file="Logger.cs" company="Trade System">
//   Copyright © Trade System 2018. All rights reserved. Confidential.
// </copyright>
//---------------------------------------------------------------------------------------------------------------------

#region Usings

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using Common.Logging;

#endregion

namespace TradeSystem
{
    /// <summary>
    /// The technology independent logger of the application. Logger instances can be added by the <see cref="AddLogger"/> method.
    /// </summary>
    public static class Logger
    {
	    private class LoggerInstance
	    {
			public ILog Logger { get; set; }
			public string[] FilePathInclude { get; set; }
			public string[] FilePathExclude { get; set; }
	    }

        private static List<LoggerInstance> loggers;
		// Caching loggers by caller file path for performance reasons
	    private static readonly ConcurrentDictionary<string, List<ILog>> loggersCache =
		    new ConcurrentDictionary<string, List<ILog>>();

		/// <summary>
		/// Gets or sets a callback, which is called when the content of an object is needed to be dumped created by the <see cref="ToContentPresenter"/> method.
		/// If not set, a default logic will be used.
		/// </summary>
		public static Func<object, string> DisplayContent { get; set; }

	    /// <summary>
	    /// Adds a logger to the application.
	    /// </summary>
	    /// <param name="logger">The logger instance to add.</param>
	    /// <param name="filePathInclude">Include filter for file path.</param>
	    /// <param name="filePathExclude">Exclude filter for file path.</param>
	    public static void AddLogger(ILog logger, string[] filePathInclude = null, string[] filePathExclude = null)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (loggers == null)
                Interlocked.CompareExchange(ref loggers, new List<LoggerInstance>(), null);

            loggers.Add(new LoggerInstance()
            {
				Logger = logger,
				FilePathInclude = filePathInclude,
				FilePathExclude = filePathExclude
            });
	        loggersCache.Clear();
		}

	    /// <summary>
	    /// Log a message object with the <see cref="F:Common.Logging.LogLevel.Trace" /> level including
	    /// the stack trace of the <see cref="T:System.Exception" /> passed
	    /// as a parameter.
	    /// </summary>
	    /// <param name="message">The message object to log.</param>
	    /// <param name="exception">The exception to log, including its stack trace.</param>
	    /// <param name="cfp">Auto injected caller file path info for filtering purpose.</param>
	    public static void Trace(object message, Exception exception = null, [CallerFilePath] string cfp = null) => Log(cfp, l => l.Trace(message, exception));

		/// <summary>
		/// Log a message with the <see cref="F:Common.Logging.LogLevel.Trace" /> level using a callback to obtain the message
		/// </summary>
		/// <remarks>
		/// Using this method avoids the cost of creating a message and evaluating message arguments
		/// that probably won't be logged due to loglevel settings.
		/// </remarks>
		/// <param name="formatMessageCallback">A callback used by the logger to obtain the message if log level is matched</param>
		/// <param name="exception">The exception to log, including its stack trace.</param>
		/// <param name="cfp">Auto injected caller file path info for filtering purpose.</param>
		public static void Trace(Action<FormatMessageHandler> formatMessageCallback, Exception exception = null, [CallerFilePath] string cfp = null) => Log(cfp, l => l.Trace(formatMessageCallback, exception));

		/// <summary>
		/// Log a message object with the <see cref="F:Common.Logging.LogLevel.Debug" /> level including
		/// the stack trace of the <see cref="T:System.Exception" /> passed
		/// as a parameter.
		/// </summary>
		/// <param name="message">The message object to log.</param>
		/// <param name="exception">The exception to log, including its stack trace.</param>
		/// <param name="cfp">Auto injected caller file path info for filtering purpose.</param>
		public static void Debug(object message, Exception exception = null, [CallerFilePath] string cfp = null) => Log(cfp, l => l.Debug(message, exception));

		/// <summary>
		/// Log a message with the <see cref="F:Common.Logging.LogLevel.Debug" /> level using a callback to obtain the message
		/// </summary>
		/// <remarks>
		/// Using this method avoids the cost of creating a message and evaluating message arguments
		/// that probably won't be logged due to loglevel settings.
		/// </remarks>
		/// <param name="formatMessageCallback">A callback used by the logger to obtain the message if log level is matched</param>
		/// <param name="exception">The exception to log, including its stack trace.</param>
		/// <param name="cfp">Auto injected caller file path info for filtering purpose.</param>
		public static void Debug(Action<FormatMessageHandler> formatMessageCallback, Exception exception = null, [CallerFilePath] string cfp = null) => Log(cfp, l => l.Debug(formatMessageCallback, exception));

		/// <summary>
		/// Log a message object with the <see cref="F:Common.Logging.LogLevel.Info" /> level including
		/// the stack trace of the <see cref="T:System.Exception" /> passed
		/// as a parameter.
		/// </summary>
		/// <param name="message">The message object to log.</param>
		/// <param name="exception">The exception to log, including its stack trace.</param>
		/// <param name="cfp">Auto injected caller file path info for filtering purpose.</param>
		public static void Info(object message, Exception exception = null, [CallerFilePath] string cfp = null) => Log(cfp, l => l.Info(message, exception));

		/// <summary>
		/// Log a message with the <see cref="F:Common.Logging.LogLevel.Info" /> level using a callback to obtain the message
		/// </summary>
		/// <remarks>
		/// Using this method avoids the cost of creating a message and evaluating message arguments
		/// that probably won't be logged due to loglevel settings.
		/// </remarks>
		/// <param name="formatMessageCallback">A callback used by the logger to obtain the message if log level is matched</param>
		/// <param name="exception">The exception to log, including its stack trace.</param>
		/// <param name="cfp">Auto injected caller file path info for filtering purpose.</param>
		public static void Info(Action<FormatMessageHandler> formatMessageCallback, Exception exception = null, [CallerFilePath] string cfp = null) => Log(cfp, l => l.Info(formatMessageCallback, exception));

		/// <summary>
		/// Log a message object with the <see cref="F:Common.Logging.LogLevel.Warn" /> level including
		/// the stack trace of the <see cref="T:System.Exception" /> passed
		/// as a parameter.
		/// </summary>
		/// <param name="message">The message object to log.</param>
		/// <param name="exception">The exception to log, including its stack trace.</param>
		/// <param name="cfp">Auto injected caller file path info for filtering purpose.</param>
		public static void Warn(object message, Exception exception = null, [CallerFilePath] string cfp = null) => Log(cfp, l => l.Warn(message, exception));

		/// <summary>
		/// Log a message with the <see cref="F:Common.Logging.LogLevel.Warn" /> level using a callback to obtain the message
		/// </summary>
		/// <remarks>
		/// Using this method avoids the cost of creating a message and evaluating message arguments
		/// that probably won't be logged due to loglevel settings.
		/// </remarks>
		/// <param name="formatMessageCallback">A callback used by the logger to obtain the message if log level is matched</param>
		/// <param name="exception">The exception to log, including its stack trace.</param>
		/// <param name="cfp">Auto injected caller file path info for filtering purpose.</param>
		public static void Warn(Action<FormatMessageHandler> formatMessageCallback, Exception exception = null, [CallerFilePath] string cfp = null) => Log(cfp, l => l.Warn(formatMessageCallback, exception));

		/// <summary>
		/// Log a message object with the <see cref="F:Common.Logging.LogLevel.Error" /> level including
		/// the stack trace of the <see cref="T:System.Exception" /> passed
		/// as a parameter.
		/// </summary>
		/// <param name="message">The message object to log.</param>
		/// <param name="exception">The exception to log, including its stack trace.</param>
		/// <param name="cfp">Auto injected caller file path info for filtering purpose.</param>
		public static void Error(object message, Exception exception = null, [CallerFilePath] string cfp = null) => Log(cfp, l => l.Error(message, exception));

		/// <summary>
		/// Log a message with the <see cref="F:Common.Logging.LogLevel.Error" /> level using a callback to obtain the message
		/// </summary>
		/// <remarks>
		/// Using this method avoids the cost of creating a message and evaluating message arguments
		/// that probably won't be logged due to loglevel settings.
		/// </remarks>
		/// <param name="formatMessageCallback">A callback used by the logger to obtain the message if log level is matched</param>
		/// <param name="exception">The exception to log, including its stack trace.</param>
		/// <param name="cfp">Auto injected caller file path info for filtering purpose.</param>
		public static void Error(Action<FormatMessageHandler> formatMessageCallback, Exception exception = null, [CallerFilePath] string cfp = null) => Log(cfp, l => l.Error(formatMessageCallback, exception));

		/// <summary>
		/// Log a message object with the <see cref="F:Common.Logging.LogLevel.Fatal" /> level including
		/// the stack trace of the <see cref="T:System.Exception" /> passed
		/// as a parameter.
		/// </summary>
		/// <param name="message">The message object to log.</param>
		/// <param name="exception">The exception to log, including its stack trace.</param>
		/// <param name="cfp">Auto injected caller file path info for filtering purpose.</param>
		public static void Fatal(object message, Exception exception = null, [CallerFilePath] string cfp = null) => Log(cfp, l => l.Fatal(message, exception));

		/// <summary>
		/// Log a message with the <see cref="F:Common.Logging.LogLevel.Fatal" /> level using a callback to obtain the message
		/// </summary>
		/// <remarks>
		/// Using this method avoids the cost of creating a message and evaluating message arguments
		/// that probably won't be logged due to loglevel settings.
		/// </remarks>
		/// <param name="formatMessageCallback">A callback used by the logger to obtain the message if log level is matched</param>
		/// <param name="exception">The exception to log, including its stack trace.</param>
		/// <param name="cfp">Auto injected caller file path info for filtering purpose.</param>
		public static void Fatal(Action<FormatMessageHandler> formatMessageCallback, Exception exception, [CallerFilePath] string cfp = null) => Log(cfp, l => l.Fatal(formatMessageCallback, exception));

        /// <summary>
        /// Returns a class, whose <see cref="Object.ToString"/> method dumps the content of the provided class. The default implementation displays the top-level public properties
        /// but this can be overridden by setting the <see cref="DisplayContent"/> delegate.
        /// </summary>
        /// <param name="obj">The object whose content should be dumped.</param>
        /// <param name="message">A message, by which the object content will be prefixed in the log.</param>
        /// <returns></returns>
        public static object ToContentPresenter(object obj, string message = null) => new ContentPresenter(obj, message);

	    private static void Log(string cfp, Action<ILog> action)
	    {
		    loggersCache.GetOrAdd(cfp, LoggersByCallerFilePath).ForEach(action);
	    }

	    private static List<ILog> LoggersByCallerFilePath(string cfp)
	    {
		    if (loggers?.Any() != true) return new List<ILog>();
		    return loggers
			    .Where(l => l.FilePathInclude == null || !l.FilePathInclude.Any() || l.FilePathInclude.Any(fp => cfp.Contains(fp ?? "")))
			    .Where(l => l.FilePathExclude == null || !l.FilePathExclude.Any() || !l.FilePathExclude.Any(fp => cfp.Contains(fp ?? "")))
			    .Select(l => l.Logger)
			    .ToList();
	    }

	    private class ContentPresenter
        {
            private readonly object obj;
            private readonly string message;
            private string dumpedContent;

            internal ContentPresenter(object obj, string message)
            {
                this.obj = obj;
                this.message = message;
            }

            public override string ToString()
            {
                if (dumpedContent == null)
                {
                    Func<object, string> dump = DisplayContent ?? DumpDefault;
                    dumpedContent = $"{(message == null ? null : $"{message} - ")}{dump.Invoke(obj)}";
                }

                return dumpedContent;
            }

            private static string DumpDefault(object obj)
            {
                if (obj == null)
                    return "<null>";
                PropertyInfo[] properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                return '{' + String.Join("; ", properties.Select(p => $"{p.Name} = {p.GetValue(obj)?.ToString() ?? "<null>"}")) + '}';
            }
        }
    }
}
