using System;
using System.Threading;
using Google.ProtocolBuffers;
using cTraderApi;

public class cTraderApiTest
{
  public const string host = "tradeapi.spotware.com";
  static cTraderClient client; 
  static long positionID;
  static void OnLogin(ProtoOAAuthRes auto)
  {
    Console.WriteLine("Login cTraderClient event handler!");
//    client.AccountsInfoAsync("https://api.spotware.com");//host);
    client.AccountsInfoAsync(host);
  }
  static void OnAccountsInfo(string json)
  {
    Console.WriteLine("AccountsInfoJSON = {0}\r\n", json);
    foreach(Account acc in client.Accounts) {
      Console.WriteLine("Account.ID = {0} ({1}:{2})", acc.ID, acc.BrokerName, acc.Number);
      client.SendSubscribeForTradingEventsRequest(acc.ID);  
      client.SendSubscribeForSpotsRequest(acc.ID, "EURUSD");
    }
  }
  static void OnTick(ProtoOASpotEvent tick)
  {
    Console.WriteLine("{0} Bid:{1} Ask:{2}", tick.SymbolName,
    (tick.HasBidPrice ? tick.BidPrice.ToString() : "       "),
    (tick.HasAskPrice ? tick.AskPrice.ToString() : "       "));
  }
  static void OnOrder(ProtoOAOrder order)
  {
    Console.WriteLine("Object order {0}", order.ToString());
  }
  static void OnPosition(ProtoOAPosition position)
  {
    positionID = position.PositionId;
    Console.WriteLine("Object position {0}", position.ToString());
  }
  static void OnError(ProtoErrorRes error)
  {
    string description = (error.HasDescription ? error.Description : "");
    Console.WriteLine("Error: {0} description: {1}", error.ErrorCode, description);
  }
  public static void Main(string[] args)
  {
//    client = new cTraderClient("S4l3LaJNzCWUT49UCMDXQqYbHhrnPNIljZJ7sM6z1eA");//, true);
    client = new cTraderClient("wIZQawuktdAe8ffUdXZzBSFqRWK9KIHlQWQYJMpPnV4");//, true);
    if(client != null) {
      client.OnLogin += OnLogin;
      client.OnTick += OnTick;
      client.OnOrder += OnOrder;
      client.OnPosition += OnPosition;
      client.OnError += OnError;
      client.OnAccountsInfo += OnAccountsInfo;

//      client.Connect(host, 5032, "182_y8Mken2PllHixdRN3xOPHlHFRDhf0ZdYqo2ItACCjUnGFb79Hh",
//                     "oBzydz12RkIbDFB6hwyHpM7HFujqhYXsI9gOibGbO1I1NS3byY");
      client.Connect(host, 5032, "228_2X7hKUyWc63STxvBiCib26uEfai8r30NSY4SBIwoAIueImtiBj",
                     "JTXVu5I8nTOa68lGWoAyx1eTWgnIrhNHiQFyVp8LlL3OppUIuZ");
      Console.Read();
/*
      client.SendMarketOrderRequest(client.Accounts[0].ID, "EURUSD",
                                                           cTraderApi.ProtoTradeSide.BUY, 1000000);
      Console.Read();
      Console.Read();
      client.SendClosePositionRequest(client.Accounts[0].ID, positionID, 1000000);
      Console.Read();
      Console.Read();
      Console.Read();
*/
      client.ConnectShutdown = true;
    }
  }
}
