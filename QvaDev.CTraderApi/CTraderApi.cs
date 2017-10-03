using System;
using System.Collections;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

namespace QvaDev.CTraderApi
{
    public class Account
    {
        public long Id { get; set; }
        public long Number { get; set; }
        public long Leverage { get; set; }
        public long Balance { get; set; }
        public string BrokerName { get; set; }
        public string BrokerTitle { get; set; }
        public string DepositCurrency { get; set; }
    }

    public class CTraderClient
    {
        public  bool ConnectShutdown
        {
            get => _connectShutdown;
            set => _connectShutdown = value;
        }
        
        private const uint MsgTimeout = 20;
        static readonly DateTime MsgTimestamp = DateTime.Now.AddSeconds(MsgTimeout);
        private const int MaxSiz = 1000000;

        static bool _debugConsole;
        static volatile bool _connectShutdown;

        static SslStream _sslStream;
        static readonly MessagesFactory InMsgFactory = new MessagesFactory();
        static readonly MessagesFactory OutMsgFactory = new MessagesFactory();

        static readonly Queue WriteQueue = new Queue();
        static readonly Queue ReadQueue = new Queue();
        static readonly Queue WriteQueueSync = Queue.Synchronized(WriteQueue);
        static readonly Queue ReadQueueSync = Queue.Synchronized(ReadQueue);

        static Thread _parseThread;
        static Thread _pingThread;
        static Thread _inputThread;
        static Thread _outputThread;

//-------------------------------------------------------------------------------------------------
//////////////////////////////////////////// THREADS //////////////////////////////////////////////
//-------------------------------------------------------------------------------------------------
        // pinging thread
        static void Ping()
        {
            _connectShutdown = false;
            while (!_connectShutdown)
            {
                Thread.Sleep(1000);

                if (DateTime.Now > MsgTimestamp)
                {
                    SendPingRequest();
                }
            }
        }

//-------------------------------------------------------------------------------------------------
        public static string GetHexadecimal(byte[] byteArray)
        {
            var hex = new StringBuilder(byteArray.Length * 2);
            foreach (var b in byteArray)
                hex.AppendFormat("{0:X2} ", b);
            return hex.ToString();
        }

//-------------------------------------------------------------------------------------------------
        // listening thread
        static void Listen(SslStream sslStream, Queue messagesQueue)
        {
            _connectShutdown = false;
            var _length = new byte[sizeof(int)];
            while (!_connectShutdown)
            {
                Thread.Sleep(1);

                int readBytes = 0;
                do
                {
                    Thread.Sleep(0);
                    readBytes += sslStream.Read(_length, readBytes, _length.Length - readBytes);
                } while (readBytes < _length.Length);

                int length = BitConverter.ToInt32(_length.Reverse().ToArray(), 0);
                if (length <= 0)
                    continue;

                if (length > MaxSiz)
                    throw new IndexOutOfRangeException();

                byte[] message = new byte[length];
                if (_debugConsole) Console.WriteLine("Data received: {0}", GetHexadecimal(_length));
                readBytes = 0;
                do
                {
                    Thread.Sleep(0);
                    readBytes += sslStream.Read(message, readBytes, message.Length - readBytes);
                } while (readBytes < length);
                if (_debugConsole) Console.WriteLine("Data received: {0}", GetHexadecimal(message));

                messagesQueue.Enqueue(message);
            }
        }

//-------------------------------------------------------------------------------------------------
        // sending thread
        static void Transmit(SslStream sslStream, Queue messagesQueue)
        {
            _connectShutdown = false;
            while (!_connectShutdown)
            {
                Thread.Sleep(1);

                if (messagesQueue.Count <= 0)
                    continue;

                byte[] message = (byte[]) messagesQueue.Dequeue();
                byte[] length = BitConverter.GetBytes(message.Length).Reverse().ToArray();

                sslStream.Write(length);
                if (_debugConsole) Console.WriteLine("Data sent: {0}", GetHexadecimal(length));
                sslStream.Write(message);
                if (_debugConsole) Console.WriteLine("Data sent: {0}", GetHexadecimal(message));
            }
        }

//-------------------------------------------------------------------------------------------------
        // received data parsing thread
        private void IncomingDataProcessing(MessagesFactory msgFactory, Queue messagesQueue)
        {
            _connectShutdown = false;
            while (!_connectShutdown)
            {
                Thread.Sleep(0);

                if (messagesQueue.Count <= 0)
                    continue;

                byte[] message = (byte[]) messagesQueue.Dequeue();
                ProcessIncomingDataStream(msgFactory, message);
            }
        }

//-------------------------------------------------------------------------------------------------
        private void ProcessIncomingDataStream(MessagesFactory msgFactory, byte[] rawData)
        {
            var msg = msgFactory.GetMessage(rawData);

            if (_debugConsole)
                Console.WriteLine($"ProcessIncomingDataStream() Message received:\n{MessagesPresentation.ToString(msg)}");

            if (_connectShutdown || !msg.HasPayload)
            {
                return;
            }

            switch (msg.PayloadType)
            {
                case (int) ProtoPayloadType.PING_REQ:
                    SendPingResponse();
                    break;
                case (int) ProtoOAPayloadType.OA_SPOT_EVENT:
                    var spot = ProtoOASpotEvent.CreateBuilder().MergeFrom(msg.Payload).Build();
                    OnTick?.Invoke(spot);
                    break;
                case (int) ProtoOAPayloadType.OA_EXECUTION_EVENT:
                    var payloadMsg = msgFactory.GetExecutionEvent(rawData);
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
                    break;
                case (int) ProtoOAPayloadType.OA_AUTH_RES:
                    var aut = ProtoOAAuthRes.CreateBuilder().MergeFrom(msg.Payload).Build();
                    OnLogin?.Invoke(aut);
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
        static void SendPingRequest()
        {
            var msg = OutMsgFactory.CreatePingRequest((ulong) DateTime.Now.Ticks);
            if (_debugConsole)
                Console.WriteLine("SendPingRequest() Message to be send:\n{0}", MessagesPresentation.ToString(msg));
            WriteQueueSync.Enqueue(msg.ToByteArray());
        }

//-------------------------------------------------------------------------------------------------
        static void SendPingResponse()
        {
            var msg = OutMsgFactory.CreatePingResponse((ulong) DateTime.Now.Ticks);
            if (_debugConsole)
                Console.WriteLine("SendPingResponse() Message to be send:\n{0}", MessagesPresentation.ToString(msg));
            WriteQueueSync.Enqueue(msg.ToByteArray());
        }

//-------------------------------------------------------------------------------------------------
        public void SendAuthorizationRequest(string clientId, string secret)
        {
            var msg = OutMsgFactory.CreateAuthorizationRequest(clientId, secret);
            if (_debugConsole)
                Console.WriteLine("SendAuthorizationRequest() Message to be send:\n{0}",
                    MessagesPresentation.ToString(msg));
            WriteQueueSync.Enqueue(msg.ToByteArray());
        }

//-------------------------------------------------------------------------------------------------
        public void SendSubscribeForSpotsRequest(string accessToken, long accountId, string symbol, string comment = null)
        {
            var msg = OutMsgFactory.CreateSubscribeForSpotsRequest(accountId, accessToken, symbol, comment);
            if (_debugConsole)
                Console.WriteLine("SendSubscribeForSpotsRequest() Message to be send:\n{0}",
                    MessagesPresentation.ToString(msg));
            WriteQueueSync.Enqueue(msg.ToByteArray());
        }

//-------------------------------------------------------------------------------------------------
        public void SendSubscribeForTradingEventsRequest(string accessToken, long accountId)
        {
            var msg = OutMsgFactory.CreateSubscribeForTradingEventsRequest(accountId, accessToken);
            if (_debugConsole)
                Console.WriteLine("SendSubscribeForTradingEventsRequest() Message to be send:\n{0}",
                    MessagesPresentation.ToString(msg));
            WriteQueueSync.Enqueue(msg.ToByteArray());
        }

//-------------------------------------------------------------------------------------------------
        public void SendUnsubscribeForTradingEventsRequest(long accountId)
        {
            var msg = OutMsgFactory.CreateUnsubscribeForTradingEventsRequest(accountId);
            if (_debugConsole)
                Console.WriteLine("SendUnsubscribeForTradingEventsRequest() Message to be send:\n{0}",
                    MessagesPresentation.ToString(msg));
            WriteQueueSync.Enqueue(msg.ToByteArray());
        }

//-------------------------------------------------------------------------------------------------
        public void SendMarketOrderRequest(string accessToken, long accountId, string symbol, ProtoTradeSide type, long volume, string clientMsgId = null)
        {
            var msg = OutMsgFactory.CreateMarketOrderRequest(accountId, accessToken, symbol, type, volume, clientMsgId);
            if (_debugConsole)
                Console.WriteLine("SendMarketOrderRequest() Message to be send:\n{0}",
                    MessagesPresentation.ToString(msg));
            WriteQueueSync.Enqueue(msg.ToByteArray());
        }

//-------------------------------------------------------------------------------------------------
        public void SendMarketRangeOrderRequest(string accessToken, long accountId, string symbol, ProtoTradeSide type, long volume,
            double price, int range, string clientMsgId = null)
        {
            var msg = OutMsgFactory.CreateMarketRangeOrderRequest(accountId, accessToken, symbol, type, volume, price, range, clientMsgId);
            if (_debugConsole)
                Console.WriteLine("SendMarketRangeOrderRequest() Message to be send:\n{0}",
                    MessagesPresentation.ToString(msg));
            WriteQueueSync.Enqueue(msg.ToByteArray());
        }

//-------------------------------------------------------------------------------------------------
        public void SendLimitOrderRequest(string accessToken, long accountId, string symbol, ProtoTradeSide type, long volume, double price,
            string clientMsgId = null)
        {
            var msg = OutMsgFactory.CreateLimitOrderRequest(accountId, accessToken, symbol, type, volume, price, clientMsgId);
            if (_debugConsole)
                Console.WriteLine("SendLimitOrderRequest() Message to be send:\n{0}",
                    MessagesPresentation.ToString(msg));
            WriteQueueSync.Enqueue(msg.ToByteArray());
        }

//-------------------------------------------------------------------------------------------------
        public void SendStopOrderRequest(string accessToken, long accountId, string symbol, ProtoTradeSide type, long volume, double price,
            string clientMsgId = null)
        {
            var msg = OutMsgFactory.CreateStopOrderRequest(accountId, accessToken, symbol, type, volume, price, clientMsgId);
            if (_debugConsole)
                Console.WriteLine("SendStopOrderRequest() Message to be send:\n{0}",
                    MessagesPresentation.ToString(msg));
            WriteQueueSync.Enqueue(msg.ToByteArray());
        }

//-------------------------------------------------------------------------------------------------
        public void SendClosePositionRequest(string accessToken, long accountId, long position, long volume, string clientMsgId = null)
        {
            var msg = OutMsgFactory.CreateClosePositionRequest(accountId, accessToken, position, volume, clientMsgId);
            if (_debugConsole)
                Console.WriteLine("SendClosePositionRequest() Message to be send:\n{0}",
                    MessagesPresentation.ToString(msg));
            WriteQueueSync.Enqueue(msg.ToByteArray());
        }

//-------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------
        public CTraderClient(bool debugConsole = false)
        {
            _debugConsole = debugConsole;

            _parseThread = new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                try
                {
                    IncomingDataProcessing(InMsgFactory, ReadQueueSync);
                }
                catch (Exception e)
                {
                    Console.WriteLine("DataProcessor throws exception: {0}", e);
                }
            });

            _inputThread = new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                try
                {
                    Listen(_sslStream, ReadQueueSync);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Listener throws exception: {0}", e);
                }
            });

            _outputThread = new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                try
                {
                    Transmit(_sslStream, WriteQueueSync);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Transmitter throws exception: {0}", e);
                }
            });

            _pingThread = new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                try
                {
                    Ping();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Listener throws exception: {0}", e);
                }
            });

        }

//-------------------------------------------------------------------------------------------------
        static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain,
            SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None) return true;
            if (_debugConsole) Console.WriteLine("Certificate error: {0}", sslPolicyErrors);
            return false;
        }

//-------------------------------------------------------------------------------------------------
        public bool Connect(string host, int port = 5032, string clientId = null, string secret = null)
        {
            try
            {
                var client = new TcpClient(host, port);
                _sslStream = new SslStream(client.GetStream(), false, ValidateServerCertificate, null);
                _sslStream.AuthenticateAsClient(host);
                _parseThread.Start();
                //_pingThread.Start();
                _inputThread.Start();
                _outputThread.Start();
            }
            catch (Exception e)
            {
                if (_debugConsole) Console.WriteLine("Establishing SSL connection error: {0}", e);
                return false;
            }
            if (clientId != null && secret != null) SendAuthorizationRequest(clientId, secret);
            return true;
        }

//-------------------------------------------------------------------------------------------------
    }
}
