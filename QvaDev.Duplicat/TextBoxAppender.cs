using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Repository.Hierarchy;

namespace QvaDev.Duplicat
{
	public class TextBoxAppender : IAppender
	{
		private class LogQueueEntry
		{
			public TextBoxAppender Appender { get; set; }
			public LoggingEvent LoggingEvent { get; set; }
		}

		public string LoggerNameFilter { get; set; }

		private RichTextBox _textBox;
		private readonly List<string> _filters;
		private readonly int _maxLines;

		private static readonly ConcurrentQueue<LogQueueEntry> LogQueue = new ConcurrentQueue<LogQueueEntry>();

		public string Name { get; set; }

		static TextBoxAppender()
		{
			var logThread = new Thread(LoggingLoop) { Name = "Logging", IsBackground = true };
			logThread.Start();
		}

		public static void ConfigureTextBoxAppender(RichTextBox textBox, string loggerNameFilter, int maxLines, params string[] filters)
		{
			var hierarchy = (Hierarchy)LogManager.GetRepository();
			var appender = new TextBoxAppender(textBox, maxLines, filters) { LoggerNameFilter = loggerNameFilter };
			hierarchy.Root.AddAppender(appender);
		}

		public TextBoxAppender(RichTextBox textBox, int maxLines, params string[] filters)
		{
			Name = "TextBoxAppender";
			_maxLines = maxLines;
			_filters = (filters ?? new string[] { }).ToList();
			_textBox = textBox;

			var frm = textBox.FindForm();
			if (frm == null) return;
			frm.FormClosing += (sender, args) => Close();
		}

		public void Close()
		{
			if (_textBox == null || _textBox.IsDisposed)
				return;

			_textBox?.Dispose();
			_textBox = null;

			var hierarchy = (Hierarchy)LogManager.GetRepository();
			hierarchy.Root.RemoveAppender(this);
		}

		public void DoAppend(LoggingEvent loggingEvent) => LogQueue.Enqueue(new LogQueueEntry { Appender = this, LoggingEvent = loggingEvent });

		private static void LoggingLoop()
		{
			while (true)
			{
				while (LogQueue.TryDequeue(out var le)) Append(le);
				Thread.Sleep(10);
			}
		}

		private static void Append(LogQueueEntry entry)
		{
			if (entry?.Appender == null || entry.LoggingEvent == null) return;

			var le = entry.LoggingEvent;
			var app = entry.Appender;

			if (app._textBox == null || app._textBox.IsDisposed) return;

			if (!string.IsNullOrWhiteSpace(app.LoggerNameFilter) && !le.LoggerName.StartsWith(app.LoggerNameFilter))
				return;

			if (le.LoggerName.Contains("NHibernate"))
				return;

			if (app._filters.Any(f => le.RenderedMessage?.Contains(f) == true))
				return;

			var msg = $"{le.TimeStamp:yyyy-MM-dd HH:mm:ss,fff} [{le.ThreadName}] {le.RenderedMessage}{Environment.NewLine}";
			if (le.ExceptionObject != null)
				msg += le.ExceptionObject + Environment.NewLine;

			Action<TextBoxAppender, Level, string> write = WriteLogEntry;
			if (app._textBox.InvokeRequired)
				app._textBox.BeginInvoke(write, app, le.Level, msg);
			else write.Invoke(app, le.Level, msg);
		}

		private static void WriteLogEntry(TextBoxAppender app, Level level, string message)
		{
			if (app._textBox == null || app._textBox.IsDisposed)
				return;

			if (app._textBox.Lines.Length > app._maxLines)
				app._textBox.Clear();

			Color color;
			if (level == Level.Trace) color = Color.Gray;
			else if (level == Level.Debug) color = Color.Black;
			else if (level == Level.Info) color = Color.Blue;
			else if (level == Level.Warn) color = Color.Olive;
			else if (level == Level.Error) color = Color.Red;
			else if (level == Level.Fatal) color = Color.Maroon;
			else color = SystemColors.WindowText;

			var selStart = app._textBox.SelectionStart;
			var selLength = app._textBox.SelectionLength;
			var resetSelection = selStart != app._textBox.TextLength;

			app._textBox.SelectionStart = app._textBox.TextLength;
			app._textBox.SelectionLength = 0;
			app._textBox.SelectionColor = color;
			app._textBox.AppendText(message);

			if (!resetSelection) return;
			app._textBox.SelectionStart = selStart;
			app._textBox.SelectionLength = selLength;
		}
	}
}
