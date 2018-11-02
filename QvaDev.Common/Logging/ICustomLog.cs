using System;

namespace QvaDev.Common.Logging
{
	public interface ICustomLog : log4net.ILog
	{
		void Trace(object message, Exception exception);
		void Trace(object message);
		void Verbose(object message, Exception exception);
		void Verbose(object message);
	}
}
