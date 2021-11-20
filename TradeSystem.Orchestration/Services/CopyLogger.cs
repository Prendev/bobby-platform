using TradeSystem.Common.Integration;
using TradeSystem.Data.Models;

namespace TradeSystem.Orchestration.Services
{
	public static class CopyLogger
	{
		public static void Log(Slave slave, string symbol, Sides side, NewPositionActions action, decimal size, decimal fill, string comment = "")
		{
			Logger.Warn($"\t{slave.Master}\t" +
			            $"\t{slave}\t" +
			            $"{symbol}\t" +
			            $"{side}\t" +
			            $"{action}\t" +
						$"{size}\t" +
			            $"{size - fill}\t" +
			            (comment ?? ""));
		}
	}
}
