using System;
using System.Windows.Forms;
using log4net;
using log4net.Appender;
using log4net.Repository.Hierarchy;

namespace QvaDev.Duplicat
{
    public class TextBoxAppender : IAppender
    {
        private TextBox _textBox;
        private readonly object _lockObj = new object();

        public TextBoxAppender(TextBox textBox)
        {
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

        public static void ConfigureTextBoxAppender(TextBox textBox)
        {
            var hierarchy = (Hierarchy) LogManager.GetRepository();
            var appender = new TextBoxAppender(textBox);
            hierarchy.Root.AddAppender(appender);
        }

        public void Close()
        {
            try
            {
                lock (_lockObj)
                {
                    _textBox = null;
                }

                var hierarchy = (Hierarchy) LogManager.GetRepository();
                hierarchy.Root.RemoveAppender(this);
            }
            catch
            {
            }
        }

        public void DoAppend(log4net.Core.LoggingEvent loggingEvent)
        {
            try
            {
                if (_textBox == null)
                    return;
                if (loggingEvent.LoggerName.Contains("NHibernate"))
                    return;
                var msg = string.Concat(
                    $"{loggingEvent.TimeStamp:yyyy-MM-dd HH:mm:ss,fff} [{loggingEvent.ThreadName}] {loggingEvent.RenderedMessage}", "\r\n");

                lock (_lockObj)
                {
                    if (_textBox == null)
                        return;
                    var del = new Action<string>(s => _textBox.AppendText(s));
                    _textBox.BeginInvoke(del, msg);
                }
            }
            catch
            {
            }
        }
    }
}
