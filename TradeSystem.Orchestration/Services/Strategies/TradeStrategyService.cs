using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TradeSystem.Data;
using TradeSystem.Data.Models;
using TradeSystem.Common.Integration;

namespace TradeSystem.Orchestration.Services.Strategies
{
	public interface ITradeStrategyService
	{
		void Start(DuplicatContext duplicatContext, int throttlingInSec);
		void Stop();
		void SetThrottling(int throttlingInSec);
		Task TradePositionClose(DuplicatContext duplicatContext, TradePosition metaTraderPosition);
		Task TradePositionRotate(DuplicatContext duplicatContext, TradePosition metaTraderPosition);
	}
	public class TradeStrategyService : BaseStrategyService, ITradeStrategyService
	{
		private volatile CancellationTokenSource _cancellation;
		private readonly SemaphoreSlim closeOrderSemaphoreSlim;
		private readonly SemaphoreSlim rotateOrderSemaphoreSlim;

		public TradeStrategyService()
		{
			closeOrderSemaphoreSlim = new SemaphoreSlim(1, 1);
			rotateOrderSemaphoreSlim = new SemaphoreSlim(1, 1);
		}

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
					var connectedAccounts = duplicatContext.Accounts.Local.Where(a => a?.Connector?.IsConnected != null && a.Connector.IsConnected && (a.MetaTraderAccount != null || a.FixApiAccount != null)).ToList();

					var metaTraderPositions = connectedAccounts
						.SelectMany(cma => cma.Connector.Positions
							.Where(p => !p.Value.IsClosed)
							.Select(p => new TradePosition
							{
								Account = cma,
								OpenTime = p.Value.OpenTime.ToString("yyyy.MM.dd. HH:mm:ss"),
								Type = p.Value.Side.ToString(),
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

					var positionsToClose = positions.Where(mtap => mtap.IsPreOrderClosing && mtap.Account.MarginLevel < mtap.MarginLevel).ToList();

					foreach (var position in positionsToClose)
					{
						await TradePositionClose(duplicatContext, position);
					}
				}
				catch (OperationCanceledException)
				{
					break;
				}
				catch (Exception e)
				{
					Logger.Error("TradesService.Loop exception", e);
				}

				await Task.Delay(_throttlingInSec * 1000);
			}
		}

		public async Task TradePositionClose(DuplicatContext duplicatContext, TradePosition metaTraderPosition)
		{
			metaTraderPosition.IsRemoved = true;

			await closeOrderSemaphoreSlim.WaitAsync();

			try
			{
				if (metaTraderPosition.Account.Connector is IMtConnector mt4Connector)
				{
					var pos = mt4Connector.Positions.FirstOrDefault(p => p.Key == metaTraderPosition.PositionKey);

					if (pos.Value != null)
					{
						var res = mt4Connector.SendClosePositionRequests(pos.Value);

						if (res.Pos.IsClosed)
						{
							var tp = duplicatContext.TraderPositions.Local.First(t => t.Id == metaTraderPosition.Id);
							duplicatContext.TraderPositions.Remove(tp);
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
							var tp = duplicatContext.TraderPositions.Local.First(t => t.Id == metaTraderPosition.Id);
							duplicatContext.TraderPositions.Remove(tp);
						}
						else metaTraderPosition.IsRemoved = false;
					}
				}
				else
				{
					metaTraderPosition.IsRemoved = false;
					Logger.Warn($"Trade is not removable because the account's connector type is {metaTraderPosition.Account.Connector.GetType()}.");
				}

				await duplicatContext.SaveChangesAsync();
			}
			finally
			{
				closeOrderSemaphoreSlim.Release();
			}
		}

		public async Task TradePositionRotate(DuplicatContext duplicatContext, TradePosition metaTraderPosition)
		{
			await TradePositionClose(duplicatContext, metaTraderPosition);
			if (metaTraderPosition.IsRemoved == false) return;

			await rotateOrderSemaphoreSlim.WaitAsync();
			try
			{
				if (metaTraderPosition.Account.Connector is IMtConnector mtConnector)
				{
					var pos = mtConnector.Positions.FirstOrDefault(p => p.Key == metaTraderPosition.PositionKey).Value;
					if (pos == null) return;

					var newPos = mtConnector.SendMarketOrderRequest(pos.Symbol, pos.Side,
					(double)pos.Lots, (int)pos.MagicNumber, pos.Comment, 0, 0);

					var copierPositions = duplicatContext.CopierPositions.Where(s => s.MasterTicket == metaTraderPosition.PositionKey).ToList();
					copierPositions.ForEach(copierPosition =>
					{
						copierPosition.MasterTicket = newPos.Pos.Id;
						copierPosition.State = CopierPosition.CopierPositionStates.Active;
					});

					duplicatContext.CopierPositions.UpdateRange(copierPositions);
				}
				else if (metaTraderPosition.Account.Connector is FixApiIntegration.Connector fixApiConnector)
				{
					var pos = fixApiConnector.Positions.FirstOrDefault(p => p.Key == metaTraderPosition.PositionKey).Value;
					if (pos == null) return;

					var newPos = await fixApiConnector.SendMarketOrderRequest(pos.Symbol, pos.Side, pos.Lots);
					if (!newPos.OrderIds.Any() || !long.TryParse(newPos.OrderIds.First(), out long orderId)) return;

					var copierPositions = duplicatContext.CopierPositions.Where(s => s.MasterTicket == metaTraderPosition.PositionKey).ToList();
					copierPositions.ForEach(copierPosition =>
					{
						copierPosition.MasterTicket = orderId;
						copierPosition.State = CopierPosition.CopierPositionStates.Active;
					});

					duplicatContext.CopierPositions.UpdateRange(copierPositions);
				}

				await duplicatContext.SaveChangesAsync();
			}
			finally
			{
				rotateOrderSemaphoreSlim.Release();
			}
		}
	}
}
