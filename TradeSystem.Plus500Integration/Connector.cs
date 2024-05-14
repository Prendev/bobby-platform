using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using TradeSystem.Common.Integration;
using TradeSystem.Common.Services;

namespace TradeSystem.Plus500Integration
{

	public class Connector : ConnectorBase
	{
		private readonly AccountInfo _accountInfo;
		private readonly IEmailService _emailService;

		private readonly System.Timers.Timer _timer;

		public override int Id => _accountInfo?.DbId ?? 0;
		public override string Description => _accountInfo?.Description;

		public override bool IsConnected => SocketIOClient?.Connected == true;

		public SocketIOClient.SocketIO SocketIOClient;

		public ConcurrentDictionary<long, PositionResponse> Plus500Positions { get; protected set; } =
			new ConcurrentDictionary<long, PositionResponse>();

		public Connector(
			AccountInfo accountInfo,
			IEmailService emailService)
		{
			_emailService = emailService;
			_accountInfo = accountInfo;
		}


		public async Task Connect()
		{
			try
			{
				if (!Uri.TryCreate($"http://{_accountInfo.SrvPath}", UriKind.Absolute, out Uri ip)) return;

				SocketIOClient = new SocketIOClient.SocketIO(ip);
				SocketIOClient.OnConnected += SocketIOClient_OnConnected;
				
				var connectTask = SocketIOClient.ConnectAsync();
				var timeoutTask = Task.Delay(10000);

				var completedTask = await Task.WhenAny(connectTask, timeoutTask);

				if(completedTask == timeoutTask) throw new TimeoutException();

				OnConnectionChanged(IsConnected ? ConnectionStates.Connected : ConnectionStates.Error);
				if (!IsConnected) return;

				SocketIOClient.On("account", ChechkMargin);
				SocketIOClient.On("order-update", GetPositions);

				//_timer.Start();
			}
			catch (TimeoutException e)
			{
				OnConnectionChanged(ConnectionStates.Error);
				Logger.Error("Connection to server timed out after 10 seconds");
			}
			catch (Exception e)
			{
				Logger.Error($"{Description} IConnector account FAILED to connect", e);
			}
		}

		private async void SocketIOClient_OnConnected(object sender, EventArgs e)
		{
			var message = new
			{
				clientId = _accountInfo.ClientId,
			};
			string jsonMessage = JsonConvert.SerializeObject(message);


			await SocketIOClient.EmitAsync("subscribe", jsonMessage);
			Logger.Debug($"Subscribed to metrics for account: {_accountInfo.Description}");
		}

		public override async void Disconnect()
		{
			try
			{
				//_timer.Stop();

				await SocketIOClient.DisconnectAsync();
			}
			catch (Exception e)
			{
				Logger.Error($"{Description} Plus500 account ERROR during disconnect", e);
			}

			OnConnectionChanged(ConnectionStates.Disconnected);
		}

		public override Tick GetLastTick(string symbol)
		{
			//TODO
			Logger.Warn("Should be got tick??");
			return new Tick();
		}

		public override void Subscribe(string symbol)
		{
			//TODO
			Logger.Warn("Should be subscribed to anything??");
		}

		private void ChechkMargin(SocketIOClient.SocketIOResponse data)
		{
			var jsonResponse = data?.ToString();

			var response = JsonConvert.DeserializeObject<AccountResponse[]>(jsonResponse);
			if (response == null || !response.Any()) return;

			Equity = response[0].Equity;
			PnL = response[0].PnL;
			Margin = Equity != 0 && response[0].AvailableBalance != 0 ? Equity - response[0].AvailableBalance : 0;
			
			FreeMargin = Equity - Margin;
			Balance = Equity - PnL;
			MarginLevel = Math.Round(Margin != 0 ? Equity / Margin * 100 : 0, 2);

			OnMarginChanged();
		}

		private void GetPositions(SocketIOClient.SocketIOResponse data)
		{
			var jsonResponse = data?.ToString();

			var response = JsonConvert.DeserializeObject<PositionResponse[]>(jsonResponse);
			if (response == null || !response.Any()) return;
			
			var position = response[0];

			Plus500Positions.AddOrUpdate(position.Id, key => position, (key, old) => position);
			//Equity = response[0].Equity;
			//PnL = response[0].PnL;
			//Margin = Equity != 0 && response[0].AvailableBalance != 0 ? Equity - response[0].AvailableBalance : 0;

			//FreeMargin = Equity - Margin;
			//Balance = Equity - PnL;
			//MarginLevel = Math.Round(Margin != 0 ? Equity / Margin * 100 : 0, 2);

		}

		private decimal M(string symbol)
		{
			return 1;
		}
	}
}
