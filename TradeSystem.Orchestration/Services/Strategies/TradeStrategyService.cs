using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TradeSystem.Data;
using TradeSystem.Data.Models;

namespace TradeSystem.Orchestration.Services.Strategies
{
	public interface ITradeStrategyService
	{
		void Start(DuplicatContext duplicatContext, int throttlingInSec);
		void Stop();
		void SetThrottling(int throttlingInSec);
		Task TradePositionClose(BindingList<TradePosition> metaTraderPositions, TradePosition metaTraderPosition);
	}
	public class TradeStrategyService : BaseStrategyService, ITradeStrategyService
	{
		private volatile CancellationTokenSource _cancellation;

		public void Start(DuplicatContext duplicatContext, int throttlingInSec)
		{
			_throttlingInSec = throttlingInSec;
			_cancellation?.Dispose();

			_cancellation = new CancellationTokenSource();
			Task.Run(() => SetLoop(duplicatContext, _cancellation.Token), _cancellation.Token);

			Logger.Info("Trade strategy's positions monitoring are started");
		}

		public void Stop()
		{
			_cancellation?.Cancel(true);
			Logger.Info("Trade strategy's positions monitoring are stopped");
		}

		private async Task SetLoop(DuplicatContext duplicatContext, CancellationToken token)
		{
			while (!token.IsCancellationRequested)
			{
				try
				{
					var connectedAccounts = duplicatContext.Accounts.Local.Where(a => a.Connector?.IsConnected == true && (a.MetaTraderAccount != null || a.FixApiAccount != null)).ToList();

					var metaTraderPositions = connectedAccounts
						.SelectMany(cma => cma.Connector.Positions
							.Where(p => !p.Value.IsClosed)
							.Select(p => new TradePosition
							{
								Account = cma,
								OpenTime = p.Value.OpenTime.ToString("yyyy.MM.dd. HH:mm:ss"),
								PositionKey = p.Key,
								Size = p.Value.Lots,
								Symbol = p.Value.Symbol,
								Comment = p.Value.Comment
							}))
						.ToList();

					var positions = duplicatContext.TraderPositions.Local.Where(mtp => connectedAccounts.Contains(mtp.Account)).ToList();

					var connectedMtAccountPositionTradeDb = positions.Where(mtp => connectedAccounts.Contains(mtp.Account)).ToList();

					var newMtPositionTrades = metaTraderPositions.Where(mtp => !connectedMtAccountPositionTradeDb.Any(mtap => mtap.Account == mtp.Account && mtap.PositionKey == mtp.PositionKey)).ToList();
					var removeMtPositionTrades = connectedMtAccountPositionTradeDb.Where(mtp => !metaTraderPositions.Any(mtap => mtap.Account == mtp.Account && mtap.PositionKey == mtp.PositionKey)).ToList();

					duplicatContext.TraderPositions.AddRange(newMtPositionTrades);
					duplicatContext.TraderPositions.RemoveRange(removeMtPositionTrades);

					await duplicatContext.SaveChangesAsync();

					foreach (var position in positions.Where(mtap => mtap.IsPreOrderClosing && mtap.Account.MarginLevel < mtap.MarginLevel).ToList())
					{
						await TradePositionClose(duplicatContext.TraderPositions.Local.ToBindingList(), position);
					}

					Thread.Sleep(_throttlingInSec * 1000);
				}
				catch (OperationCanceledException)
				{
					break;
				}
				catch (Exception e)
				{
					Logger.Error("TradesService.Loop exception", e);
				}
			}
		}

		public async Task TradePositionClose(BindingList<TradePosition> metaTraderPositions, TradePosition metaTraderPosition)
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
					var res = await fixApiConnector.CloseOrderRequest(pos.Value.Symbol, pos.Value.Side, pos.Value.Lots, 1000, 1, 5, new[] { pos.Key.ToString() });
					if (res.OrderIds != null && res.OrderIds.Any(orderId => orderId.Equals(pos.Key.ToString())))
					{
						metaTraderPositions.Remove(metaTraderPosition);
					}
					else metaTraderPosition.IsRemoved = false;
				}
			}
		}
	}
}
