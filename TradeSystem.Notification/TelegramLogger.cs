using System;

namespace TradeSystem.Notification
{
	public static class TelegramLogger
	{
		public static void Debug(string message)
		{
			Logger.Debug(message);
		}
		public static void Info(string message)
		{
			Logger.Info(message);
		}
		public static void Warn(string message)
		{
			Logger.Warn(message);
		}
		public static void Error(string message, Exception e = null)
		{
			Logger.Error(message, e);
		}
	}
}
