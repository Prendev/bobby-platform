using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using TradeSystem.Data;
using TradeSystem.Data.Models;

namespace TradeSystem.Orchestration.Services.Strategies
{
	public interface ITradeStrategyService
	{
		Task RefreshOpenedPosition(DuplicatContext duplicatContext);
		Task TradePositionClose(BindingList<MetaTraderPosition> metaTraderPositions, MetaTraderPosition metaTraderPosition);
	}
	public class TradeStrategyService : ITradeStrategyService
	{
		public async Task RefreshOpenedPosition(DuplicatContext duplicatContext)
		{
			//TODO - maybe I should add to be mt5 for fixapi
			var connectedAccounts = duplicatContext.Accounts.Local.Where(a => a.Connector?.IsConnected == true && (a.MetaTraderAccount != null || a.FixApiAccount != null)).ToList();

			var metaTraderPositions = connectedAccounts
				.SelectMany(cma => cma.Connector.Positions
					.Where(p => !p.Value.IsClosed)
					.Select(p => new MetaTraderPosition
					{
						Account = cma,
						OpenTime = p.Value.OpenTime.ToString("yyyy.MM.dd. HH:mm:ss"),
						PositionKey = p.Key,
						Size = p.Value.Lots,
						Symbol = p.Value.Symbol,
						Comment = p.Value.Comment
					}))
				.ToList();

			var positions = duplicatContext.MetaTraderPositions.Local.Where(mtp => connectedAccounts.Contains(mtp.Account)).ToList();

			var connectedMtAccountPositionTradeDb = positions.Where(mtp => connectedAccounts.Contains(mtp.Account)).ToList();

			var newMtPositionTrades = metaTraderPositions.Where(mtp => !connectedMtAccountPositionTradeDb.Any(mtap => mtap.Account == mtp.Account && mtap.PositionKey == mtp.PositionKey)).ToList();
			var removeMtPositionTrades = connectedMtAccountPositionTradeDb.Where(mtp => !metaTraderPositions.Any(mtap => mtap.Account == mtp.Account && mtap.PositionKey == mtp.PositionKey)).ToList();

			duplicatContext.AddRange(newMtPositionTrades);
			duplicatContext.RemoveRange(removeMtPositionTrades);

			duplicatContext.SaveChanges();

			foreach (var position in positions.Where(mtap => mtap.IsPreOrderClosing && mtap.Account.MarginLevel < mtap.MarginLevel).ToList())
			{
				await TradePositionClose(duplicatContext.MetaTraderPositions.Local.ToBindingList(), position);
			}
		}

		public async Task TradePositionClose(BindingList<MetaTraderPosition> metaTraderPositions, MetaTraderPosition metaTraderPosition)
		{
			metaTraderPosition.IsRemoved = true;

			if (metaTraderPosition.Account.Connector is Mt4Integration.Connector mt4Connector)
			{
				var pos = mt4Connector.Positions.FirstOrDefault(p => p.Key == metaTraderPosition.PositionKey);

				if (pos.Value != null)
				{
					var res = mt4Connector.SendClosePositionRequests(pos.Value);

					if (res.Pos.IsClosed)
					{
						metaTraderPositions.Remove(metaTraderPosition);
					}
					else metaTraderPosition.IsRemoved = false;
				}
			}
			else if (metaTraderPosition.Account.Connector is FixApiIntegration.Connector fixApiConnector)
			{
				var pos = fixApiConnector.Positions.FirstOrDefault(p => p.Key == metaTraderPosition.PositionKey);

				if (pos.Value != null)
				{
					//string symbol, Sides side, decimal quantity, int timeout, int retryCount, int retryPeriod, string[] orderIds
					var res = await fixApiConnector.CloseOrderRequest(pos.Value.Symbol, pos.Value.Side, pos.Value.Lots, 10000000, 2, 2, new[] { pos.Key.ToString() });
					//if (res.Pos.IsClosed)
					//{
					//	MtPositions.Remove(mtPosition);
					//	ConnectedMtPositions.Remove(mtPosition);
					//}
					//else mtPosition.IsRemoved = false;
				}
			}
		}
	}
}
