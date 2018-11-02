using System;
using log4net;

namespace QvaDev.Common.Logging
{
	public interface ILogExtended : ILog
	{
		void Trace(object message, Exception exception);
		void Trace(object message);
		void Verbose(object message, Exception exception);
		void Verbose(object message);
	}
}
