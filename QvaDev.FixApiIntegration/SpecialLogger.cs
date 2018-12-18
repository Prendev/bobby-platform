using QvaDev.Communication.FixApi;

namespace QvaDev.FixApiIntegration
{
	public static class SpecialLogger
	{
		public static void Log(Connector connector, ExecutionReportEventArgs e)
		{
			if ((e.ExecutionReport.FulfilledQuantity ?? 0) == 0) return;
			if (e.ExecutionReport.Side != Side.Buy && e.ExecutionReport.Side != Side.Sell) return;
			Logger.Debug($"\t{connector.Description}\t{e.ExecutionReport.Symbol}\t{e.ExecutionReport.Side}\t{e.ExecutionReport.FulfilledQuantity}\t{e.ExecutionReport.FulfilledPrice}");
		}
	}
}
