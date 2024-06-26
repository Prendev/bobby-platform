﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Repository.Hierarchy;

namespace TradeSystem.Duplicat
{
	public class TextBoxAppender : IAppender
	{
		public string LoggerNameFilter { get; set; }

		private RichTextBox _textBox;
		private readonly List<string> _filters;
		private readonly int _maxLines;
		private readonly bool _logLevelColoring;

		public TextBoxAppender(RichTextBox textBox, int maxLines, params string[] filters)
		{
			bool.TryParse(ConfigurationManager.AppSettings["LogLevelColoring"], out bool logLevelColoring);
			_logLevelColoring = logLevelColoring;
			_maxLines = maxLines;
			_filters = (filters ?? new string[] { }).ToList();

			var frm = textBox.FindForm();
			if (frm == null)
				return;

			frm.FormClosing += delegate
			{
				Close();
			};

			_textBox = textBox;
			Name = "TextBoxAppender";
		}

		public string Name { get; set; }

		public static void ConfigureTextBoxAppender(RichTextBox textBox, string loggerNameFilter, int maxLines, params string[] filters)
		{
			var hierarchy = (Hierarchy)LogManager.GetRepository();
			var appender = new TextBoxAppender(textBox, maxLines, filters) { LoggerNameFilter = loggerNameFilter };
			hierarchy.Root.AddAppender(appender);
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

		public void DoAppend(LoggingEvent loggingEvent)
		{
			if (loggingEvent.Level.Value < Level.Debug.Value) return;
			if (_textBox == null || _textBox.IsDisposed)
				return;

			if (!string.IsNullOrWhiteSpace(LoggerNameFilter) && !LoggerNameFilter.Split('|').Contains(loggingEvent.LoggerName))
				return;

			if (loggingEvent.LoggerName.Contains("NHibernate"))
				return;

			if (_filters.Any(f => loggingEvent.RenderedMessage?.Contains(f) == true))
				return;

			var msg = $"{loggingEvent.RenderedMessage}{Environment.NewLine}";
			if (loggingEvent.ExceptionObject != null)
				msg += loggingEvent.ExceptionObject + Environment.NewLine;

			Action<Level, string> write = WriteLogEntry;
			_textBox.BeginInvoke(write, loggingEvent.Level, msg);

			// No need for checking as it is always called from dedicated Logging thread
			//if (_textBox.InvokeRequired)
			//	_textBox.BeginInvoke(write, loggingEvent.Level, msg);
			//else
			//	write.Invoke(loggingEvent.Level, msg);
		}

		private void WriteLogEntry(Level level, string message)
		{
			if (_textBox == null || _textBox.IsDisposed)
				return;

			if (_textBox.IsDisposed) return;
			if (_textBox.Lines.Length > _maxLines) _textBox.Clear();

			if (_logLevelColoring)
			{
				Color color;
				if (level == Level.Trace) color = Color.Gray;
				else if (level == Level.Debug) color = Color.Black;
				else if (level == Level.Info) color = Color.Blue;
				else if (level == Level.Warn) color = Color.Olive;
				else if (level == Level.Error) color = Color.Red;
				else if (level == Level.Fatal) color = Color.Maroon;
				else color = SystemColors.WindowText;

				var selStart = _textBox.SelectionStart;
				var selLength = _textBox.SelectionLength;
				var resetSelection = selStart != _textBox.TextLength;

				_textBox.SelectionStart = _textBox.TextLength;
				_textBox.SelectionLength = 0;
				_textBox.SelectionColor = color;
				_textBox.AppendText(message);

				if (!resetSelection) return;
				_textBox.SelectionStart = selStart;
				_textBox.SelectionLength = selLength;
			}
			else _textBox.AppendText(message);
		}
	}
}
