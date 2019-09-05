using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TradeSystem.Common.Integration;
using TradeSystem.Common.Services;
using TradingAPI.MT4Server;

namespace TradeSystem.Mt4Integration
{
	public interface IConnector : Common.Integration.IConnector
	{
		PositionResponse SendMarketOrderRequest(string symbol, Sides side, double lots, int magicNumber,
			string comment, int maxRetryCount, int retryPeriodInMs);

		bool SendClosePositionRequests(Position position, double? lots, int maxRetryCount, int retryPeriodInMs);

		ConcurrentDictionary<long, Position> Positions { get; }
	}

	public class Connector : ConnectorBase, IConnector
	{
		//private readonly HashSet<int> _finishedOrderIds = new HashSet<int>();
		private readonly List<string> _symbols = new List<string>();
		private readonly ConcurrentDictionary<string, SymbolInfo> _symbolInfos =
            new ConcurrentDictionary<string, SymbolInfo>();
        private readonly ConcurrentDictionary<string, Tick> _lastTicks =
            new ConcurrentDictionary<string, Tick>();
		private readonly IEmailService _emailService;
		private AccountInfo _accountInfo;

	    public override int Id => _accountInfo?.DbId ?? 0;
		public override string Description => _accountInfo?.Description;
		public override bool IsConnected => QuoteClient?.Connected == true && OrderClient?.Connected == true;
	    public DateTime? ServerTime => QuoteClient?.ServerTime;
		public ConcurrentDictionary<long, Position> Positions { get; } = new ConcurrentDictionary<long, Position>();

		public QuoteClient QuoteClient;
        public OrderClient OrderClient;
		private Action<string, int> _destinationSetter;

		public Connector(IEmailService emailService)
		{
			_emailService = emailService;
		}

		public override void Disconnect()
        {
            QuoteClient.OnDisconnect -= QuoteClient_OnDisconnect;
            QuoteClient.OnOrderUpdate -= QuoteClient_OnOrderUpdate;
	        QuoteClient.OnQuote -= QuoteClient_OnQuote;

			try
			{
				QuoteClient?.Disconnect();
				OrderClient?.Disconnect();
			}
	        catch (Exception e)
	        {
		        Logger.Error($"{Description} account ERROR during disconnect", e);
			}

			OnConnectionChanged(ConnectionStates.Disconnected);
		}

        public void Connect(AccountInfo accountInfo, Action<string, int> destinationSetter)
        {
	        _destinationSetter = destinationSetter;
	        _accountInfo = accountInfo;

            Server[] slaves = null;
			try
			{
				if (Uri.TryCreate($"http://{_accountInfo.Srv}", UriKind.Absolute, out Uri ip))
					QuoteClient = CreateQuoteClient(_accountInfo, ip.Host, ip.IsDefaultPort ? 443 : ip.Port);
				else
				{
					var srv = QuoteClient.LoadSrv(_accountInfo.Srv, out slaves);
					QuoteClient = CreateQuoteClient(_accountInfo, srv.Host, srv.Port);
				}
				QuoteClient.Connect();
			}
			catch (Exception e)
			{
				Logger.Error($"{_accountInfo.Description} account ({_accountInfo.User}) FAILED to connect", e);
			}
			finally
			{
				if (QuoteClient?.Connected != true) ConnectSlaves(slaves, _accountInfo);
			}

	        OrderClient = new OrderClient(QuoteClient);
	        OrderClient.Connect();

			OnConnectionChanged(IsConnected ? ConnectionStates.Connected : ConnectionStates.Error);
			if (!IsConnected) return;

            QuoteClient.OnOrderUpdate -= QuoteClient_OnOrderUpdate;
            QuoteClient.OnOrderUpdate += QuoteClient_OnOrderUpdate;
            QuoteClient.OnDisconnect -= QuoteClient_OnDisconnect;
            QuoteClient.OnDisconnect += QuoteClient_OnDisconnect;
	        QuoteClient.OnQuote -= QuoteClient_OnQuote;
	        QuoteClient.OnQuote += QuoteClient_OnQuote;

	        lock (_symbols)
	        {
		        foreach (var symbol in _symbols)
		        {
			        if (QuoteClient.IsSubscribed(symbol)) return;
			        QuoteClient.Subscribe(symbol);
		        }
	        }

			foreach (var o in QuoteClient.GetOpenedOrders().Where(o => o.Type == Op.Buy || o.Type == Op.Sell))
            {
                var pos = new Position
                {
                    Id = o.Ticket,
                    Lots = (decimal) o.Lots,
                    Symbol = o.Symbol,
                    Side = o.Type == Op.Buy ? Sides.Buy : Sides.Sell,
                    RealVolume = (long) (o.Lots * GetSymbolInfo(o.Symbol).ContractSize * (o.Type == Op.Buy ? 1 : -1)),
                    MagicNumber = o.MagicNumber,
                    Profit = o.Profit,
                    Commission = o.Commission,
                    Swap = o.Swap,
                    OpenTime = o.OpenTime,
                    OpenPrice = (decimal)o.OpenPrice,
                    Comment = o.Comment
                };
                Positions.AddOrUpdate(o.Ticket, key => pos, (key, old) =>
                {
                    pos.CloseOrder = old.CloseOrder;
                    return pos;
                });
			}
		}

		public PositionResponse SendMarketOrderRequest(string symbol, Sides side, double lots, int magicNumber,
			string comment, int maxRetryCount, int retryPeriodInMs)
        {
			var retValue = new PositionResponse();
            try
			{
				var op = side == Sides.Buy ? Op.Buy : Op.Sell;
				var o = OrderClient.OrderSend(symbol, op, lots, 0, 0, 0, 0, comment, magicNumber, DateTime.MaxValue);
				Logger.Debug($"{_accountInfo.Description} Connector.SendMarketOrderRequest({symbol}, {side}, {lots}, {magicNumber}, {comment}) is successful with id {o.Ticket}");

				var position = new Position
				{
					Id = o.Ticket,
					Lots = (decimal) o.Lots,
					Symbol = o.Symbol,
					Side = o.Type == Op.Buy ? Sides.Buy : Sides.Sell,
					RealVolume = (long)(o.Lots * GetSymbolInfo(o.Symbol).ContractSize * (o.Type == Op.Buy ? 1 : -1)),
					MagicNumber = o.MagicNumber,
					Profit = o.Profit,
					Commission = o.Commission,
					Swap = o.Swap,
					OpenTime = o.OpenTime,
					OpenPrice = (decimal)o.OpenPrice,
					Comment = o.Comment
				};
				retValue.Pos = Positions.AddOrUpdate(position.Id, t => position, (t, old) => position);
				return retValue;
			}
            catch (TradingAPI.MT4Server.TimeoutException e)
            {
	            Logger.Error($"{_accountInfo.Description} Connector.SendMarketOrderRequest({symbol}, {side}, {lots}, {magicNumber}, {comment}) TIMEOUT exception", e);
	            retValue.IsUnfinished = true;

	            _emailService.Send("ALERT - Market order TIMEOUT",
		            $"{Description}" + Environment.NewLine +
		            $"{symbol}" + Environment.NewLine +
		            $"{side.ToString()}" + Environment.NewLine +
		            $"{lots}");
				return retValue;
            }
			catch (Exception e)
            {
                Logger.Error($"{_accountInfo.Description} Connector.SendMarketOrderRequest({symbol}, {side}, {lots}, {magicNumber}, {comment}) exception", e);
				if (maxRetryCount <= 0) return retValue;

				Thread.Sleep(retryPeriodInMs);
				return SendMarketOrderRequest(symbol, side, lots, magicNumber, comment, --maxRetryCount, retryPeriodInMs);
			}
        }

		public PositionResponse SendMarketOrderRequest(string symbol, Sides side, double lots, int magicNumber, string comment)
		{
			return SendMarketOrderRequest(symbol, side, lots, magicNumber, comment, 0, 0);
		}

		public bool SendClosePositionRequests(Position position, double? lots, int maxRetryCount, int retryPeriodInMs)
        {
	        if (position == null)
			{
				Logger.Error($"{_accountInfo.Description} Connector.SendClosePositionRequests position is NULL");
				return false;
			}

            try
			{
				var price = position.Side == Sides.Buy
                    ? QuoteClient.GetQuote(position.Symbol).Bid
                    : QuoteClient.GetQuote(position.Symbol).Ask;
				var order = OrderClient.OrderClose(position.Symbol, (int) position.Id, lots ?? (double) position.Lots, price, 0);
				UpdatePosition(order);
				Logger.Debug($"{_accountInfo.Description} Connector.SendClosePositionRequests({position.Id}, {position.Comment}) is successful");
				return true;
			}
            catch (Exception e)
            {
                Logger.Error($"{_accountInfo.Description} Connector.SendClosePositionRequests({position.Id}, {position.Comment}) exception", e);
				if (maxRetryCount <= 0) return false;

				Thread.Sleep(retryPeriodInMs);
				return SendClosePositionRequests(position, lots, --maxRetryCount, retryPeriodInMs);
			}
        }

		public bool SendClosePositionRequests(Position position, double? lots = null)
		{
			return SendClosePositionRequests(position, lots, 0, 0);
		}

	    public bool SendClosePositionRequests(long ticket, int maxRetryCount, int retryPeriodInMs)
	    {
		    if (!Positions.TryGetValue(ticket, out var position)) return true;
		    if (position.IsClosed) return true;
		    return SendClosePositionRequests(position, null, maxRetryCount, retryPeriodInMs);
	    }

		public override Tick GetLastTick(string symbol)
        {
            return _lastTicks.GetOrAdd(symbol, (Tick)null);
        }

		public override void Subscribe(string symbol)
		{
			try
			{
				lock (_symbols)
					if (!_symbols.Contains(symbol))
						_symbols.Add(symbol);

				if (QuoteClient.IsSubscribed(symbol)) return;
				QuoteClient.Subscribe(symbol);
			}
			catch (Exception e)
			{
				Logger.Error($"{_accountInfo.Description} Connector.Subscribe({symbol}) exception", e);
			}
		}

		private void QuoteClient_OnQuote(object sender, QuoteEventArgs args)
        {
            var tick = new Tick
            {
                Symbol = args.Symbol,
                Ask = (decimal)args.Ask,
                Bid = (decimal)args.Bid,
                Time = args.Time
            };
            _lastTicks.AddOrUpdate(args.Symbol, key => tick, (key, old) => tick);
            OnNewTick(new NewTick {Tick = tick});
        }

        private void QuoteClient_OnDisconnect(object sender, DisconnectEventArgs args)
        {
			OnConnectionChanged(ConnectionStates.Error);
            Logger.Error($"{_accountInfo.Description} account ({_accountInfo.User}) disconnected", args.Exception);
            while (!IsConnected)
            {
	            Connect(_accountInfo, _destinationSetter);
	            if (IsConnected) return;
	            Thread.Sleep(new TimeSpan(0, 1, 0));
			}
	        OnConnectionChanged(IsConnected ? ConnectionStates.Connected : ConnectionStates.Error);
		}

        private void QuoteClient_OnOrderUpdate(object sender, OrderUpdateEventArgs update)
		{
			var o = update.Order;
			if (!new[] {UpdateAction.PositionOpen, UpdateAction.PositionClose, UpdateAction.PendingFill}.Contains(update.Action)) return;
	        if (!new[] {Op.Buy, Op.Sell}.Contains(o.Type)) return;

	        var position = UpdatePosition(o);

            OnNewPosition(new NewPosition
            {
                AccountType = AccountTypes.Mt4,
                Position = position,
                Action = update.Action == UpdateAction.PositionClose ? NewPositionActions.Close : NewPositionActions.Open,
			});

			// TODO
			//if (update.Action != UpdateAction.PositionOpen) return;
			//Task.Run(() =>
			//{
			//	lock (_finishedOrderIds)
			//		if (_finishedOrderIds.Contains(o.Ticket))
			//			return;

			//	Logger.Error(
			//		$"{Description} QuoteClient.OnOrderUpdate unfinished order ({o.Symbol}, {o.Type}, {o.Lots})!!!");
			//	SendClosePositionRequests(o.Ticket, 5, 25);
			//});
		}
		private Position UpdatePosition(Order order)
		{
			var position = new Position
			{
				Id = order.Ticket,
				Lots = (decimal)order.Lots,
				Symbol = order.Symbol,
				Side = order.Type == Op.Buy ? Sides.Buy : Sides.Sell,
				RealVolume = (long)(order.Lots * GetSymbolInfo(order.Symbol).ContractSize * (order.Type == Op.Buy ? 1 : -1)),
				OpenTime = order.OpenTime,
				OpenPrice = (decimal)order.OpenPrice,
				CloseTime = order.CloseTime,
				ClosePrice = (decimal)order.ClosePrice,
				IsClosed = order.Ex.close_time > 0,
				MagicNumber = order.MagicNumber,
				Profit = order.Profit,
				Commission = order.Commission,
				Swap = order.Swap,
				Comment = order.Comment
			};
			return Positions.AddOrUpdate(order.Ticket, t => position, (t, old) =>
			{
				old.CloseTime = order.CloseTime;
				old.ClosePrice = (decimal) order.ClosePrice;
				old.IsClosed = order.Ex.close_time > 0;
				old.Profit = order.Profit;
				old.Commission = order.Commission;
				old.Swap = order.Swap;
				return old;
			});
		}

		private SymbolInfo GetSymbolInfo(string symbol)
        {
            return _symbolInfos.GetOrAdd(symbol, s => QuoteClient.GetSymbolInfo(symbol));
        }

        private QuoteClient CreateQuoteClient(AccountInfo accountInfo, string host, int port)
        {
	        _destinationSetter?.Invoke(host, port);
	        var client = new QuoteClient(accountInfo.User, accountInfo.Password,
		        accountInfo.LocalPortForProxy.HasValue ? "localhost" : host, accountInfo.LocalPortForProxy ?? port);

            return client;
        }

        private void ConnectSlaves(Server[] slaves, AccountInfo accountInfo)
        {
			if (Uri.TryCreate($"http://{_accountInfo.Srv}", UriKind.Absolute, out Uri ip)) return;
			if (slaves?.Any() != true) return;
            foreach (var srv in slaves)
            {
                try
				{
					QuoteClient = CreateQuoteClient(accountInfo, srv.Host, srv.Port);
                    QuoteClient.Connect();
                    if (IsConnected) return;
                }
                catch (Exception e)
                {
                    Logger.Error($"{_accountInfo.Description} account ({_accountInfo.User}) FAILED to connect", e);
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            QuoteClient?.Disconnect();
            OrderClient?.Disconnect();
        }

        ~Connector()
        {
            Dispose(false);
        }
    }
}
