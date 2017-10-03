using System;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using cTraderApi;

namespace apiTest
{
    public partial class MainForm: Form
    {
        public const string acn = "accountNumber";
        public const string acs = "accountStatus";
        public const string tat = "traderAccountType";
        public const string brn = "brokerName";
        public const string brc = "brokerCode";
        public const string brt = "brokerTitle";
        public const string trt = "traderRegistrationTimestamp";
        public const string del = "deleted";
        public const string swf = "swapFree";
        public const string liv = "live";
        public const string dec = "depositCurrency";
        public const string bal = "balance";

        public const string sym = "symbol=";
        public const string typ = "type=";
        public const string vol = "volume=";
        public const string pri = "price=";
        public const string ran = "range=";
        public const string cmt = "comment=";
        public const string pos = "position=";
        public const string ord = "order=";

        public const string htt = "https://"; 
        public const string trd = "trade"; 
        public const string pro = "/connect/profile";
        public const string acc = "/connect/tradingaccounts";
        public const string acc_deals = "/deals";
        public const string acc_cashf = "/cashflowhistory";

        public const string tok = "access_token=";

        public CheckBox debugBox = null;
        public ListBox symbolBox = null;
        public ComboBox commandBox = null;
        public TextBox param = null;      
        public ListBox resul = null;

        public static MainForm main;


//        static string host = "sandbox-tradeapi.spotware.com"; 
        static string host = "tradeapi.spotware.com";
        static int port = 5032;
//        static string clientID = "142_5JuISSAfEcg8w1Mpu2wpAbSh9DsWyxSqVe6YkNtTYSuwKLL7YL";
        static string clientID = "182_y8Mken2PllHixdRN3xOPHlHFRDhf0ZdYqo2ItACCjUnGFb79Hh";
//        static string secret = "iEwGtFhvSN7ZOwBN1EOFwQxXttXHJuzEfsMwhTcmYc8Bcoa6au";
        static string secret = "oBzydz12RkIbDFB6hwyHpM7HFujqhYXsI9gOibGbO1I1NS3byY";
//        static long accountID = 90026; // login:3004542 pass:6388 http://sandbox-ct.spotware.com
        static long accountID = 399727; // 3180391 Pepperstone
        //static long accountID = 399645; // 9965977 FxPro
//        static string access_token = "UJVYfvuuTixsNC7OXh2ooLaJosEw0VwrD5tNr4asL18";
        static string access_token = "S4l3LaJNzCWUT49UCMDXQqYbHhrnPNIljZJ7sM6z1eA";


        static uint msgTimeout = 20;
        static DateTime msgTimestamp = DateTime.Now.AddSeconds(msgTimeout);
        static int maxSiz = 1000000;

        static bool isDebugIsOn = false;
        volatile static bool mainShutdown = false;

        static SslStream sslStream;
        static MessagesFactory inMsgFactory = new MessagesFactory();
        static MessagesFactory outMsgFactory = new MessagesFactory();

        static Queue writeQueue = new Queue(); // not thear safe
        static Queue readQueue = new Queue(); // not thear safe
        static Queue writeQueueSync = Queue.Synchronized(writeQueue); // thread safe
        static Queue readQueueSync = Queue.Synchronized(readQueue); // thread safe

//-------------------------------------------------------------------------------------------------
///////////////////////////////////////////// WINDOW //////////////////////////////////////////////
//-------------------------------------------------------------------------------------------------
        public string get_par(string str) {
          if(param != null) {
            string st = "", tst = param.Text;
            int i = tst.IndexOf(str);
            if(i >= 0) {
              i += str.Length;
              while(i < tst.Length && tst[i] != '\r') {
                st += tst[i];
                i++;
              }
              if(st.Length > 0) return st;
            }
          }
          return null;
        }
//-------------------------------------------------------------------------------------------------
        public void set_par(string str, string val) {
          if(this.param != null) {
            string st = "", tst = this.param.Text;
            int i = tst.IndexOf(str);
            if(i >= 0) {
              while(i < tst.Length && tst[i] != '\r') {
                st += tst[i];
                i++;
              }
              tst = tst.Replace(st, str + val);
            } else tst += str + val + "\r\n";
            this.param.Text = tst;
          }
        }
//-------------------------------------------------------------------------------------------------
        public bool res_exists(long num, string res) {
          if(this.resul != null) {
            for(int i = 0; i < this.resul.Items.Count; i++)
            if((this.resul.Items[i] as string).IndexOf(res) == 0) {
              int fro = (res).Length - 1;
              int len = (this.resul.Items[i] as string).Length - fro + 1;
              if (num == Convert.ToInt64((this.resul.Items[i] as string).Substring(fro, len)))
              return true;
            }
          }
          return false;
        }
//-------------------------------------------------------------------------------------------------
        public long res_select(string res) {
          if(this.resul != null) {
            int i = this.resul.Items.Count - 1; // last
            if(this.resul.SelectedIndex >= 0) i = this.resul.SelectedIndex;
            if(i > 0) while((this.resul.Items[i] as string).IndexOf(res) != 0) i--;
            if(i > 0 && (this.resul.Items[i] as string).IndexOf(res) == 0) {
              int fro = (res).Length - 1;
              int len = (this.resul.Items[i] as string).Length - fro + 1;
              return Convert.ToInt64((this.resul.Items[i] as string).Substring(fro, len));
            }
          }
          return 0;
        }
//-------------------------------------------------------------------------------------------------
        public string symbol {
          get { return get_par(sym); } set { set_par(sym, value); } 
        }
        public string type {
          get { return get_par(typ); } set { set_par(typ, value); }
        }
        public string volume {
          get { return get_par(vol); } set { set_par(vol, value); }
        }
        public string price {
          get { return get_par(pri); } set { set_par(pri, value); }
        }
        public string range {
          get { return get_par(ran); } set { set_par(ran, value); }
        }
        public string comment {
          get { return get_par(cmt); } set { set_par(cmt, value); }
        }
//-------------------------------------------------------------------------------------------------
        private void ButtonClick(object Sender, EventArgs e)
        {
           if(commandBox != null && commandBox.SelectedIndex >= 0) {
             switch(commandBox.SelectedIndex)
             {
               case 0: SendPingRequest(); break;
               case 1: SendAuthorizationRequest(); break;
               case 2: SendSubscribeForSpotsRequest(); break;
               case 3: SendSubscribeForTradingEventsRequest(); break;
               case 4: SendUnsubscribeForTradingEventsRequest(); break;
               case 5: SendMarketOrderRequest(); break;
               case 6: SendMarketRangeOrderRequest(); break;
               case 7: SendLimitOrderRequest(); break;
               case 8: SendStopOrderRequest(); break;
               case 9: SendClosePositionRequest(); break;
               case 10: AccountInfoAsync(); break;
               default: break;
             }
             Thread.Sleep(700);
             Console.WriteLine("Run -> " + (commandBox.Items[commandBox.SelectedIndex] as string));
           }
        }
        private void debugBoxClick(object Sender, EventArgs e)
        {
           isDebugIsOn = (Sender as CheckBox).Checked;
        }
        private void MainFormClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
          mainShutdown = true;
          Thread.Sleep(700);
        }
//-------------------------------------------------------------------------------------------------
//////////////////////////////////////////// THREADS //////////////////////////////////////////////
//-------------------------------------------------------------------------------------------------
        // ping thread
        static void Ping()
        {
            mainShutdown = false;
            while (!mainShutdown)
            {
                Thread.Sleep(1000);

                if (DateTime.Now > msgTimestamp)
                {
                    SendPingRequest();
                }
            }
        }

//-------------------------------------------------------------------------------------------------
        // listen thread
        static void Listen(SslStream sslStream, Queue messagesQueue)
        {
            mainShutdown = false;
            while (!mainShutdown)
            {
                Thread.Sleep(1);

                byte[] _length = new byte[sizeof(int)];
                int readBytes = 0;
                do
                {
                    Thread.Sleep(0);
                    readBytes += sslStream.Read(_length, readBytes, _length.Length - readBytes);
                } while (readBytes < _length.Length);

                int length = BitConverter.ToInt32(_length.Reverse().ToArray(), 0);
                if (length <= 0)
                    continue;

                if (length > maxSiz)
                {
                    string exceptionMsg = "Message length " + length.ToString() + " is out of range (0 - " + maxSiz.ToString() + ")";
                    throw new System.IndexOutOfRangeException();
                }

                byte[] _message = new byte[length];
                if (isDebugIsOn) Console.WriteLine("Data received: {0}", GetHexadecimal(_length));
                readBytes = 0;
                do
                {
                    Thread.Sleep(0);
                    readBytes += sslStream.Read(_message, readBytes, _message.Length - readBytes);
                } while (readBytes < length);
                if (isDebugIsOn) Console.WriteLine("Data received: {0}", GetHexadecimal(_message));

                messagesQueue.Enqueue(_message);
            }
        }

//-------------------------------------------------------------------------------------------------
        // sender thread
        static void Transmit(SslStream sslStream, Queue messagesQueue, DateTime msgTimestamp)
        {
            mainShutdown = false;
            while (!mainShutdown)
            {
                Thread.Sleep(1);

                if (messagesQueue.Count <= 0)
                    continue;

                byte[] _message = (byte[])messagesQueue.Dequeue();
                byte[] _length = BitConverter.GetBytes(_message.Length).Reverse().ToArray(); ;

                sslStream.Write(_length);
                if (isDebugIsOn) Console.WriteLine("Data sent: {0}", GetHexadecimal(_length));
                sslStream.Write(_message);
                if (isDebugIsOn) Console.WriteLine("Data sent: {0}", GetHexadecimal(_message));
                msgTimestamp = DateTime.Now.AddSeconds(msgTimeout);
            }
        }

//-------------------------------------------------------------------------------------------------
        // input data processing thread
        static void IncomingDataProcessing(MessagesFactory msgFactory, Queue messagesQueue)
        {
            mainShutdown = false;
            while (!mainShutdown)
            {
                Thread.Sleep(0);

                if (messagesQueue.Count <= 0)
                    continue;

                byte[] _message = (byte[])messagesQueue.Dequeue();
                ProcessIncomingDataStream(msgFactory, _message);
            }
        }

//-------------------------------------------------------------------------------------------------
        static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;
            Console.WriteLine("Certificate error: {0}", sslPolicyErrors);
            return false;
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
        private void set_spot(uint id, string st)
        {
          if(symbolBox != null) {
            int i = 0;
            string fst = st.Substring(0, 6);
            while(i < symbolBox.Items.Count &&
            !fst.Equals((symbolBox.Items[i] as string).Substring(0, 6))) i++;
            if(i >= symbolBox.Items.Count) symbolBox.Items.Add(st); else
            {
              string tst = (symbolBox.Items[i] as string);
              string dst = "";
              for(int j = 0; j < st.Length || j < tst.Length; j++)
              if(j >= st.Length) dst += tst[j]; else
              if(j >= tst.Length) dst += st[j]; else
              dst += (st[j] != ' ' ? st[j] : tst[j]);
              symbolBox.Items[i] = dst;
            }
          }
        }

        private void set_res(long id, string st)
        {
          //if(resul != null && !res_exists(id, st)) resul.Items.Add(st + id);
          if(resul != null) resul.Items.Add(st + id);
        }

        static void ProcessIncomingDataStream(MessagesFactory msgFactory, byte[] rawData)
        {
            var _msg = msgFactory.GetMessage(rawData);

            if (isDebugIsOn) Console.WriteLine("ProcessIncomingDataStream() Message received:\n{0}", MessagesPresentation.ToString(_msg));

            if (mainShutdown || !_msg.HasPayload)
            {
                return;
            }

            if((ProtoOAPayloadType)_msg.PayloadType == ProtoOAPayloadType.OA_SPOT_EVENT) {
              var _spot_event = ProtoOASpotEvent.CreateBuilder().MergeFrom(_msg.Payload).Build();
              main.Invoke(new Action(() => main.set_spot(_spot_event.SubscriptionId, _spot_event.SymbolName +
              ":" + (_spot_event.HasBidPrice ? _spot_event.BidPrice.ToString() : "       ") +
              ":" + (_spot_event.HasAskPrice ? _spot_event.AskPrice.ToString() : "       ")
              )));
            }

            switch (_msg.PayloadType)
            {
                case (int)cTraderApi.ProtoPayloadType.PING_REQ:
                    SendPingResponse();
                    break;
                case (int)cTraderApi.ProtoPayloadType.HEARTBEAT_EVENT:
                    //SendHeartbeatEvent();
                    break;
                case (int)cTraderApi.ProtoOAPayloadType.OA_EXECUTION_EVENT:
                    var _payload_msg = msgFactory.GetExecutionEvent(rawData);
                    if (_payload_msg.HasOrder)
                    {
                        long testOrderId = _payload_msg.Order.OrderId;
                        main.Invoke(new Action(() => main.set_res(testOrderId, ord)));
                    }
                    if (_payload_msg.HasPosition)
                    {
                        long testPositionId = _payload_msg.Position.PositionId;
                        main.Invoke(new Action(() => main.set_res(testPositionId, pos)));
                    }
                    break;
                default:
                    break;
            }
        }
//-------------------------------------------------------------------------------------------------
//////////////////////////////////////////// ACCOUNT API //////////////////////////////////////////
//-------------------------------------------------------------------------------------------------
        public void AccountInfoRequest()
        {
            string url = host;
            url = htt + host.Replace(trd, "") + acc + '?' + tok + access_token;
            Console.WriteLine("Account info request send.");
            WebRequest request = WebRequest.Create(url);
            WebResponse response = request.GetResponse();
            using (Stream stream = response.GetResponseStream()) {
                using (StreamReader reader = new StreamReader(stream)) {
                    string line = "";
                    while ((line = reader.ReadLine()) != null) {
                        Console.WriteLine(line);
                    }
                }     
            }
            response.Close();
            Console.WriteLine("Account info request done.");
        }
//-------------------------------------------------------------------------------------------------
        public static async Task AccountInfoAsync()
        {
            string url = host;
            url = htt + host.Replace(trd, "") + acc + '?' + tok + access_token;
            Console.WriteLine("Account info async send. => " + url);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
            //WebHeaderCollection headers = response.Headers;
            //for(int i=0; i< headers.Count; i++)
            //Console.WriteLine("{0}: {1}", headers.GetKey(i), headers[i]);
             using (Stream stream = response.GetResponseStream()) {
               using (StreamReader reader = new StreamReader(stream)) {
                 Console.WriteLine(reader.ReadToEnd());
               }
            }
	    response.Close();
            Console.WriteLine("Account info async done.");
        }
//-------------------------------------------------------------------------------------------------
//////////////////////////////////////////// TRADING API //////////////////////////////////////////
//-------------------------------------------------------------------------------------------------
        static void SendPingRequest()
        {
            var _msg = outMsgFactory.CreatePingRequest((ulong)DateTime.Now.Ticks);
            if (isDebugIsOn) Console.WriteLine("SendPingRequest() Message to be send:\n{0}", MessagesPresentation.ToString(_msg));
            writeQueueSync.Enqueue(_msg.ToByteArray());
        }
//-------------------------------------------------------------------------------------------------
        static void SendPingResponse()
        {
            var _msg = outMsgFactory.CreatePingResponse((ulong)DateTime.Now.Ticks);
            if (isDebugIsOn) Console.WriteLine("SendPingResponse() Message to be send:\n{0}", MessagesPresentation.ToString(_msg));
            writeQueueSync.Enqueue(_msg.ToByteArray());
        }
//-------------------------------------------------------------------------------------------------
        static void SendHeartbeatEvent()
        {
            var _msg = outMsgFactory.CreateHeartbeatEvent();
            if (isDebugIsOn) Console.WriteLine("SendHeartbeatEvent() Message to be send:\n{0}", MessagesPresentation.ToString(_msg));
            writeQueueSync.Enqueue(_msg.ToByteArray());
        }
//-------------------------------------------------------------------------------------------------
        static void SendAuthorizationRequest()
        {
            var _msg = outMsgFactory.CreateAuthorizationRequest(clientID, secret);
            if (isDebugIsOn) Console.WriteLine("SendAuthorizationRequest() Message to be send:\n{0}", MessagesPresentation.ToString(_msg));
            writeQueueSync.Enqueue(_msg.ToByteArray());
        }
//-------------------------------------------------------------------------------------------------
        static void SendSubscribeForSpotsRequest()
        {
            var _msg = outMsgFactory.CreateSubscribeForSpotsRequest(accountID, access_token, main.symbol, main.comment);
            if (isDebugIsOn) Console.WriteLine("SendSubscribeForSpotsRequest() Message to be send:\n{0}", MessagesPresentation.ToString(_msg));
            writeQueueSync.Enqueue(_msg.ToByteArray());
        }
//-------------------------------------------------------------------------------------------------
        static void SendSubscribeForTradingEventsRequest()
        {
            var _msg = outMsgFactory.CreateSubscribeForTradingEventsRequest(accountID, access_token);
            if (isDebugIsOn) Console.WriteLine("SendSubscribeForTradingEventsRequest() Message to be send:\n{0}", MessagesPresentation.ToString(_msg));
            writeQueueSync.Enqueue(_msg.ToByteArray());
        }
//-------------------------------------------------------------------------------------------------
        static void SendUnsubscribeForTradingEventsRequest()
        {
            var _msg = outMsgFactory.CreateUnsubscribeForTradingEventsRequest(accountID);
            if (isDebugIsOn) Console.WriteLine("SendUnsubscribeForTradingEventsRequest() Message to be send:\n{0}", MessagesPresentation.ToString(_msg));
            writeQueueSync.Enqueue(_msg.ToByteArray());
        }
//-------------------------------------------------------------------------------------------------
        static void SendMarketOrderRequest()
        {
            var _msg = outMsgFactory.CreateMarketOrderRequest(accountID, access_token, main.symbol, (main.type == "BUY" ? cTraderApi.ProtoTradeSide.BUY : cTraderApi.ProtoTradeSide.SELL), Convert.ToInt64(main.volume), main.comment);
            if (isDebugIsOn) Console.WriteLine("SendMarketOrderRequest() Message to be send:\n{0}", MessagesPresentation.ToString(_msg));
            writeQueueSync.Enqueue(_msg.ToByteArray());
        }
//-------------------------------------------------------------------------------------------------
        static void SendMarketRangeOrderRequest()
        {
            var _msg = outMsgFactory.CreateMarketRangeOrderRequest(accountID, access_token, main.symbol, (main.type == "BUY" ? cTraderApi.ProtoTradeSide.BUY : cTraderApi.ProtoTradeSide.SELL), Convert.ToInt64(main.volume), Convert.ToDouble(main.price), Convert.ToInt32(main.range), main.comment);
            if (isDebugIsOn) Console.WriteLine("SendMarketRangeOrderRequest() Message to be send:\n{0}", MessagesPresentation.ToString(_msg));
            writeQueueSync.Enqueue(_msg.ToByteArray());
        }
//-------------------------------------------------------------------------------------------------
        static void SendLimitOrderRequest()
        {
            var _msg = outMsgFactory.CreateLimitOrderRequest(accountID, access_token, main.symbol, (main.type == "BUY" ? cTraderApi.ProtoTradeSide.BUY : cTraderApi.ProtoTradeSide.SELL), Convert.ToInt64(main.volume), Convert.ToDouble(main.price), main.comment);
            if (isDebugIsOn) Console.WriteLine("SendLimitOrderRequest() Message to be send:\n{0}", MessagesPresentation.ToString(_msg));
            writeQueueSync.Enqueue(_msg.ToByteArray());
        }
//-------------------------------------------------------------------------------------------------
        static void SendStopOrderRequest()
        {
            var _msg = outMsgFactory.CreateStopOrderRequest(accountID, access_token, main.symbol, (main.type == "BUY" ? cTraderApi.ProtoTradeSide.BUY : cTraderApi.ProtoTradeSide.SELL), Convert.ToInt64(main.volume), Convert.ToDouble(main.price), main.comment);
            if (isDebugIsOn) Console.WriteLine("SendStopOrderRequest() Message to be send:\n{0}", MessagesPresentation.ToString(_msg));
            writeQueueSync.Enqueue(_msg.ToByteArray());
        }
//-------------------------------------------------------------------------------------------------
        static void SendClosePositionRequest()
        {
            long cur_pos = 0;//main.res_select(pos);
            if(cur_pos <= 0 && (cur_pos = Convert.ToInt32(main.get_par(pos))) < 0) {
              Console.WriteLine("SendClosePositionRequest() Error, select or set param - position=?!");
              return;
            }
            var _msg = outMsgFactory.CreateClosePositionRequest(accountID, access_token, cur_pos, Convert.ToInt64(main.volume), main.comment);
            if (isDebugIsOn) Console.WriteLine("SendClosePositionRequest() Message to be send:\n{0}", MessagesPresentation.ToString(_msg));
            writeQueueSync.Enqueue(_msg.ToByteArray());
        }              
//-------------------------------------------------------------------------------------------------
////////////////////////////////////////////// MAIN ///////////////////////////////////////////////
//-------------------------------------------------------------------------------------------------
        static void Main(string[] args)
        {
          Console.WriteLine("------------------------------------------------Start");
          Console.WriteLine("SSL connection to {0}:{1}...", host, port);
          try {
            TcpClient client = new TcpClient(host, port);
            sslStream = new SslStream(client.GetStream(), false,
            new RemoteCertificateValidationCallback(ValidateServerCertificate), null);
            sslStream.AuthenticateAsClient(host);
          } catch (Exception e) {
            Console.WriteLine("Establishing SSL connection error: {0}", e);
            return;
          }
          Console.WriteLine("The connection is established successfully.");

          Thread p = new Thread(() => {
            Thread.CurrentThread.IsBackground = true;
            try {
              IncomingDataProcessing(inMsgFactory, readQueueSync);
            } catch (Exception e) {
              Console.WriteLine("DataProcessor throws exception: {0}", e);
            }
          });
          p.Start();

          Thread tl = new Thread(() => {
            Thread.CurrentThread.IsBackground = true;
            try {
              Listen(sslStream, readQueueSync);
            } catch (Exception e) {
              Console.WriteLine("Listener throws exception: {0}", e);
            }
          });
          tl.Start();

          Thread ts = new Thread(() => {
            Thread.CurrentThread.IsBackground = true;
            try {
              Transmit(sslStream, writeQueueSync, msgTimestamp);
            } catch (Exception e) {
              Console.WriteLine("Transmitter throws exception: {0}", e);
            }
          });
          ts.Start();

          Thread t = new Thread(() => {
            Thread.CurrentThread.IsBackground = true;
            try {
              Ping();
            } catch (Exception e) {
              Console.WriteLine("Listener throws exception: {0}", e);
            }
          });
          t.Start();

          main = new MainForm();
          if(main != null) {
            main.Closing += new CancelEventHandler(main.MainFormClosing);
            main.Text = "Demo:3004542 pass:6388 http://sandbox-ct.spotware.com";
            main.Location = new Point(100, 100);
            main.Size = new Size(500, 350);
            main.MaximizeBox = false;
            main.symbolBox = new ListBox();
            if(main.symbolBox != null) {
              main.Controls.Add(main.symbolBox);
              main.symbolBox.Location = new Point(10, 10);
              main.symbolBox.Size = new Size(140, 300);
              main.symbolBox.MultiColumn = true;
              main.symbolBox.SelectionMode = SelectionMode.MultiExtended;
              main.symbolBox.AutoSize = true;
            }

            main.commandBox = new ComboBox();
            if(main.commandBox != null) {
              main.Controls.Add(main.commandBox);
              main.commandBox.Location = new Point(160, 10);
              main.commandBox.Size = new Size(230, 20);

              main.commandBox.Items.Add("Send Ping Request");             // 0
              main.commandBox.Items.Add("Send Authorization Request");    // 1
              main.commandBox.Items.Add("Subscribe for Spots Request");   // 2
              main.commandBox.Items.Add("Subscribe for Trading Request"); // 3
              main.commandBox.Items.Add("Unsubscribe Trading Request");   // 4
              main.commandBox.Items.Add("Send Market Order");             // 5
              main.commandBox.Items.Add("Send Market Range Order");       // 6
              main.commandBox.Items.Add("Send Limit Order");              // 7              
              main.commandBox.Items.Add("Send Stop Order");               // 8
              main.commandBox.Items.Add("Close Position Request");        // 9
              main.commandBox.Items.Add("Account Info Request");          // 10

              main.commandBox.SelectedIndex = 0;
            }

            Button butt = new Button();
            if(butt != null) {
              main.Controls.Add(butt);
              butt.Location = new Point(400, 10);
              butt.Size = new Size(70, 20);
              butt.Text = "-> Run";
              butt.Click += new System.EventHandler(main.ButtonClick);
            }

            GroupBox grou = new GroupBox();
            if(grou != null) {
              main.Controls.Add(grou);
              grou.Location = new Point(160, 40);
              grou.Size = new Size(320, 260);
              grou.Text = "Params/Results";
              main.debugBox = new CheckBox();
              main.param = new TextBox();
              if(main.param != null) {
                grou.Controls.Add(main.param);
                main.param.Location = new Point(10, 20);
                main.param.Size = new Size(145, 200);
                main.param.Multiline = true;
                main.symbol = "USDCHF";
                main.type = "BUY";
                main.price = "1,06789";
                main.volume = "1000000";
                main.range = "5";
                main.comment = "Test";
              }
              main.resul = new ListBox();
              if(main.resul != null) {
                grou.Controls.Add(main.resul);
                main.resul.Location = new Point(165, 20);
                main.resul.Size = new Size(145, 200);
              }
              if(main.debugBox != null) {
                grou.Controls.Add(main.debugBox);
                main.debugBox.Location = new Point(10, 225);
                main.debugBox.Size = new Size(300, 30);
                main.debugBox.Text = "Printing debug I/O protocol buffers data to console";
                main.debugBox.Click += new System.EventHandler(main.debugBoxClick);
              }
            }

            SendAuthorizationRequest();
            Console.WriteLine("Authorization request...");
            Thread.Sleep(1000);
            main.symbol = "USDCHF"; SendSubscribeForSpotsRequest();
            main.symbol = "USDJPY"; SendSubscribeForSpotsRequest();
            main.symbol = "GBPUSD"; SendSubscribeForSpotsRequest();
            main.symbol = "EURUSD"; SendSubscribeForSpotsRequest();           
            Console.WriteLine("Subscribe for market data request...");
            SendSubscribeForTradingEventsRequest();
            Console.WriteLine("Subscribe for trading event request...");
            AccountInfoAsync();

            Application.Run(main);
            Console.WriteLine("------------------------------------------------End.");
          }
        } // method Main
    } // class MainForm
} // namespece apiTest
