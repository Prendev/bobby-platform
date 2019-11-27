using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TradeSystem.CTraderApi
{
    public class CTraderClient
    {
        private const int MaxSiz = 1000000;

        private readonly MessagesFactory _inMsgFactory = new MessagesFactory();
        private readonly MessagesFactory _outMsgFactory = new MessagesFactory();

        private readonly ConcurrentQueue<byte[]> _writeQueue = new ConcurrentQueue<byte[]>();
        private readonly ConcurrentQueue<byte[]> _readQueue = new ConcurrentQueue<byte[]>();

        private SslStream _sslStream;
	    private Task _pingTask;
		private Task _parseTask;
        private Task _inputTask;
        private Task _outputTask;
		private CancellationTokenSource _cancellationTokenSource;
	    private ConnectionDetails _cd;

	    public bool Debug  => _cd?.Debug ?? false;

//-------------------------------------------------------------------------------------------------
//////////////////////////////////////////// THREADS //////////////////////////////////////////////
//-------------------------------------------------------------------------------------------------
		// pinging thread
	    void Ping(object state)
		{
			Thread.Sleep(20000);
			var token = (CancellationToken)state;
			while (!token.IsCancellationRequested)
			{
			    Thread.Sleep(5000);
				SendHearthbeatRequest();
		    }
	    }

//-------------------------------------------------------------------------------------------------
		public string GetHexadecimal(byte[] byteArray)
        {
            var hex = new StringBuilder(byteArray.Length * 2);
            foreach (var b in byteArray)
                hex.AppendFormat("{0:X2} ", b);
            return hex.ToString();
        }

//-------------------------------------------------------------------------------------------------
        // listening thread
        void Listen(object state)
        {
	        var token = (CancellationToken) state;
            var lengthMessage = new byte[sizeof(int)];
			while (!token.IsCancellationRequested)
			{
				try
				{
					Thread.Sleep(1);

					var readBytes = 0;
					while (readBytes < lengthMessage.Length)
					{
						Thread.Sleep(1);
						readBytes += _sslStream.Read(lengthMessage, readBytes, lengthMessage.Length - readBytes);
					}

					var length = BitConverter.ToInt32(lengthMessage.Reverse().ToArray(), 0);
					if (length <= 0) continue;
					if (length > MaxSiz) throw new IndexOutOfRangeException();

					//if (Debug) Logger.Debug($"Data received: {GetHexadecimal(lengthMessage)}");

					var dataMessage = new byte[length];
					readBytes = 0;
					while (readBytes < length)
					{
						Thread.Sleep(1);
						readBytes += _sslStream.Read(dataMessage, readBytes, dataMessage.Length - readBytes);
					}

					//if (Debug) Logger.Debug($"Data received: {GetHexadecimal(dataMessage)}");

					_readQueue.Enqueue(dataMessage);
				}
				catch (Exception e)
				{
					Logger.Error($"{_cd?.Description} Listener throws exception: {0}", e);
					Reconnect();
				}
			}
		}

//-------------------------------------------------------------------------------------------------
        // sending thread
        void Transmit(object state)
		{
			var token = (CancellationToken)state;
			while (!token.IsCancellationRequested)
            {
                try
                {
                    Thread.Sleep(1);
                    if (!_writeQueue.TryDequeue(out var dataMessage)) continue;

                    var lengthMessage = BitConverter.GetBytes(dataMessage.Length).Reverse().ToArray();
                    _sslStream.Write(lengthMessage);
                    _sslStream.Write(dataMessage);

	                //if (Debug) Logger.Debug($"Data sent: {GetHexadecimal(lengthMessage)}");
					//if (Debug) Logger.Debug($"Data sent: {GetHexadecimal(dataMessage)}");
                }
                catch (Exception e)
                {
                    Logger.Error($"{_cd?.Description} Transmitter throws exception: {0}", e);
	                Reconnect();
                }
            }
        }

//-------------------------------------------------------------------------------------------------
        // received data parsing thread
        private void IncomingDataProcessing(object state)
		{
			var token = (CancellationToken)state;
			while (!token.IsCancellationRequested)
            {
                try
                {
                    Thread.Sleep(1);

                    byte[] message;
                    if (!_readQueue.TryDequeue(out message)) continue;

                    ProcessIncomingDataStream(message);
                }
                catch (Exception e)
                {
                    Logger.Error($"{_cd?.Description} DataProcessor throws exception: {0}", e);
                }
            }
        }

//-------------------------------------------------------------------------------------------------
        private void ProcessIncomingDataStream(byte[] rawData)
        {
            var msg = _inMsgFactory.GetMessage(rawData);

            if (Debug)
                Logger.Debug($"{_cd?.Description} ProcessIncomingDataStream() Message received:\n{MessagesPresentation.ToString(msg)}");

            if (_cancellationTokenSource.Token.IsCancellationRequested || !msg.HasPayload)
            {
                return;
            }

            switch (msg.PayloadType)
            {
                case (int) ProtoOAPayloadType.OA_SPOT_EVENT:
                    var spot = ProtoOASpotEvent.CreateBuilder().MergeFrom(msg.Payload).Build();
                    OnTick?.Invoke(spot);
                    break;
                case (int) ProtoOAPayloadType.OA_EXECUTION_EVENT:
                    var payloadMsg = _inMsgFactory.GetExecutionEvent(rawData);
                    if (payloadMsg.HasOrder)
                    {
                        ProtoOAOrder order = payloadMsg.Order;
                        OnOrder?.Invoke(order, msg.ClientMsgId);
                    }
                    if (payloadMsg.HasPosition)
                    {
                        ProtoOAPosition position = payloadMsg.Position;
                        OnPosition?.Invoke(position);
                    }
                    break;
                case (int) ProtoPayloadType.ERROR_RES:
                    var err = ProtoErrorRes.CreateBuilder().MergeFrom(msg.Payload).Build();
					OnError?.Invoke(err, msg.ClientMsgId);
	                Logger.Error($"{_cd?.Description} CTraderClient_OnError", new Exception(err.Description));
					break;
                case (int) ProtoOAPayloadType.OA_AUTH_RES:
                    var aut = ProtoOAAuthRes.CreateBuilder().MergeFrom(msg.Payload).Build();
					OnLogin?.Invoke(aut);
	                Logger.Debug($"{_cd?.Description} CTraderClient_OnLogin");
					break;
            }
        }

//-------------------------------------------------------------------------------------------------
///////////////////////////////////////////// EVENTS //////////////////////////////////////////////
//-------------------------------------------------------------------------------------------------
        public delegate void ErrorEvent(ProtoErrorRes error, string clientMsgId);

        public delegate void LoginEvent(ProtoOAAuthRes auto);

        public delegate void TickEvent(ProtoOASpotEvent tick);

        public delegate void OrderEvent(ProtoOAOrder order, string clientMsgId);

        public delegate void PositionEvent(ProtoOAPosition position);

        public event ErrorEvent OnError;
        public event LoginEvent OnLogin;
        public event TickEvent OnTick;
        public event OrderEvent OnOrder;

        public event PositionEvent OnPosition;

//-------------------------------------------------------------------------------------------------
//////////////////////////////////////////// TRADING API //////////////////////////////////////////
//-------------------------------------------------------------------------------------------------
        public void SendAuthorizationRequest(string clientId, string secret)
        {
            var msg = _outMsgFactory.CreateAuthorizationRequest(clientId, secret);
            if (Debug)
                Logger.Debug($"{_cd?.Description} SendAuthorizationRequest() Message to be send:\n{MessagesPresentation.ToString(msg)}");
            _writeQueue.Enqueue(msg.ToByteArray());
        }

//-------------------------------------------------------------------------------------------------
        public void SendSubscribeForSpotsRequest(string accessToken, long accountId, string symbol,
            string comment = null)
        {
            var msg = _outMsgFactory.CreateSubscribeForSpotsRequest(accountId, accessToken, symbol, comment);
            if (Debug)
                Logger.Debug($"{_cd?.Description} SendSubscribeForSpotsRequest() Message to be send:\n{MessagesPresentation.ToString(msg)}");
            _writeQueue.Enqueue(msg.ToByteArray());
        }

//-------------------------------------------------------------------------------------------------
        public void SendSubscribeForTradingEventsRequest(string accessToken, long accountId)
        {
            var msg = _outMsgFactory.CreateSubscribeForTradingEventsRequest(accountId, accessToken);
            if (Debug)
                Logger.Debug(
                    $"{_cd?.Description} SendSubscribeForTradingEventsRequest() Message to be send:\n{MessagesPresentation.ToString(msg)}");
            _writeQueue.Enqueue(msg.ToByteArray());
        }

//-------------------------------------------------------------------------------------------------
        public void SendUnsubscribeForTradingEventsRequest(long accountId)
        {
            var msg = _outMsgFactory.CreateUnsubscribeForTradingEventsRequest(accountId);
            if (Debug)
                Logger.Debug(
                    $"{_cd?.Description} SendUnsubscribeForTradingEventsRequest() Message to be send:\n{MessagesPresentation.ToString(msg)}");
            _writeQueue.Enqueue(msg.ToByteArray());
        }

//-------------------------------------------------------------------------------------------------
        public void SendMarketOrderRequest(string accessToken, long accountId, string symbol, ProtoTradeSide type,
            long volume, string clientMsgId = null)
        {
            var msg = _outMsgFactory.CreateMarketOrderRequest(accountId, accessToken, symbol, type, volume, clientMsgId);
            if (Debug)
                Logger.Debug($"{_cd?.Description} SendMarketOrderRequest() Message to be send:\n{MessagesPresentation.ToString(msg)}");
            _writeQueue.Enqueue(msg.ToByteArray());
        }

//-------------------------------------------------------------------------------------------------
        public void SendMarketRangeOrderRequest(string accessToken, long accountId, string symbol, ProtoTradeSide type,
            long volume,
            double price, int range, string clientMsgId = null)
        {
            var msg = _outMsgFactory.CreateMarketRangeOrderRequest(accountId, accessToken, symbol, type, volume, price,
                range, clientMsgId);
            if (Debug)
                Logger.Debug($"{_cd?.Description} SendMarketRangeOrderRequest() Message to be send:\n{MessagesPresentation.ToString(msg)}");
            _writeQueue.Enqueue(msg.ToByteArray());
        }

//-------------------------------------------------------------------------------------------------
        public void SendLimitOrderRequest(string accessToken, long accountId, string symbol, ProtoTradeSide type,
            long volume, double price,
            string clientMsgId = null)
        {
            var msg = _outMsgFactory.CreateLimitOrderRequest(accountId, accessToken, symbol, type, volume, price,
                clientMsgId);
            if (Debug)
                Logger.Debug($"{_cd?.Description} SendLimitOrderRequest() Message to be send:\n{MessagesPresentation.ToString(msg)}");
            _writeQueue.Enqueue(msg.ToByteArray());
        }

//-------------------------------------------------------------------------------------------------
        public void SendStopOrderRequest(string accessToken, long accountId, string symbol, ProtoTradeSide type,
            long volume, double price,
            string clientMsgId = null)
        {
            var msg = _outMsgFactory.CreateStopOrderRequest(accountId, accessToken, symbol, type, volume, price,
                clientMsgId);
            if (Debug)
                Logger.Debug($"{_cd?.Description} SendStopOrderRequest() Message to be send:\n{MessagesPresentation.ToString(msg)}");
            _writeQueue.Enqueue(msg.ToByteArray());
        }

//-------------------------------------------------------------------------------------------------
        public void SendClosePositionRequest(string accessToken, long accountId, long position, long volume,
            string clientMsgId = null)
        {
            var msg = _outMsgFactory.CreateClosePositionRequest(accountId, accessToken, position, volume, clientMsgId);
            if (Debug)
                Logger.Debug($"{_cd?.Description} SendClosePositionRequest() Message to be send:\n{MessagesPresentation.ToString(msg)}");
            _writeQueue.Enqueue(msg.ToByteArray());
        }

//-------------------------------------------------------------------------------------------------
        public void SendHearthbeatRequest(string clientMsgId = null)
        {
            var msg = _outMsgFactory.CreateHeartbeatEvent(clientMsgId);
			//if (Debug)
			//    Logger.Debug($"{_cd?.Description} SendHearthbeatRequest() Message to be send:\n{MessagesPresentation.ToString(msg)}");
			_writeQueue.Enqueue(msg.ToByteArray());
        }
//-------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------

        bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain,
            SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None) return true;
            if (Debug) Logger.Error($"{_cd?.Description} Certificate error: {sslPolicyErrors}");
            return false;
        }

//-------------------------------------------------------------------------------------------------
        public bool Connect(ConnectionDetails cd)
        {
	        _cd = cd;
	        _cancellationTokenSource = new CancellationTokenSource();
			_pingTask = new Task(Ping, _cancellationTokenSource.Token, _cancellationTokenSource.Token,
				TaskCreationOptions.LongRunning);
			_parseTask = new Task(IncomingDataProcessing, _cancellationTokenSource.Token, _cancellationTokenSource.Token,
		        TaskCreationOptions.LongRunning);
	        _inputTask = new Task(Listen, _cancellationTokenSource.Token, _cancellationTokenSource.Token,
		        TaskCreationOptions.LongRunning);
	        _outputTask = new Task(Transmit, _cancellationTokenSource.Token, _cancellationTokenSource.Token,
		        TaskCreationOptions.LongRunning);

            try
            {
                var client = new TcpClient(cd.TradingHost, cd.Port);
                _sslStream = new SslStream(client.GetStream(), false, ValidateServerCertificate, null);
                _sslStream.AuthenticateAsClient(cd.TradingHost);
                _sslStream.Flush();

	            _pingTask.Start();
				_parseTask.Start();
                _inputTask.Start();
                _outputTask.Start();

	            SendAuthorizationRequest(cd.ClientId, cd.Secret);
			}
            catch (Exception e)
            {
                Logger.Error($"{cd.Description} Establishing SSL connection error: {0}", e);
                return false;
			}
	        Logger.Debug($"{cd.Description} cTrader platform connected");
			return true;
        }

        public void Disconnect()
        {
            _cancellationTokenSource.Cancel();
	        _sslStream?.Dispose();
		}

		public void Reconnect()
		{
			Logger.Debug($"{_cd.Description} cTrader platform reconnect...");
			Disconnect();
			Connect(_cd);
		}

		//-------------------------------------------------------------------------------------------------
	}
}
