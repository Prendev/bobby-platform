using System;
using System.Threading;
using NUnit.Framework;
using TradeSystem.Common.Integration;
using TradeSystem.Data;
using TradeSystem.Data.Models;
using TradeSystem.Orchestration.Services;

namespace TradeSystem.OrchestrationTests.Services
{
	[TestFixture]
	public class SpoofingServiceTests
	{
		private Spoof Spoof { get; set; }
		private SpoofingService SpoofingService { get; set; }

		[SetUp]
		public void SetUp()
		{
			SpoofingService = new SpoofingService();

			var connectorFactory = new ConnectorFactory(null, null);
			var feedAccount = new Account()
			{
				Run = true,
				IbAccount = new IbAccount()
				{
					Description = "Feed",
					Port = 7496,
					ClientId = 1
				},
				IbAccountId = 1
			};
			var tradeAccount = new Account()
			{
				Run = true,
				CqgClientApiAccount = new CqgClientApiAccount()
				{
					Description = "Trade",
					Id = 1,
					UserName = "SIgor-ZGM41"
				},
				CqgClientApiAccountId = 1
			};
			connectorFactory.Create(feedAccount).Wait();
			connectorFactory.Create(tradeAccount).Wait();
			Spoof = new Spoof(feedAccount, "FUT|DTB|FDAX DEC 18", tradeAccount, "F.US.DDZ18", 1, 1, 1, 0, null);

			Assert.IsTrue(feedAccount.Connector.IsConnected);
			Assert.IsTrue(tradeAccount.Connector.IsConnected);

			Spoof.FeedAccount.Connector.Subscribe(Spoof.FeedSymbol);
		}

		[TearDown]
		public void TearDown()
		{
			Spoof.FeedAccount?.Connector?.Disconnect();
			Spoof.TradeAccount?.Connector?.Disconnect();
		}

		[Test]
		public void SpoofTest()
		{
			// Act
			var state = SpoofingService.Spoofing(Spoof, Sides.Buy);

			// Assert
			Thread.Sleep(new TimeSpan(0, 0, 5));
			state.Cancel().Wait();
		}

		[Test]
		public void ManualTest()
		{
			// Arrange
			return;
			var fixConnector = ((IFixConnector) Spoof.TradeAccount.Connector);

			// Act
			//SpoofingService.Spoofing(spoof, Sides.Buy, cancel.Token);
			var response = fixConnector.SendSpoofOrderRequest(Spoof.TradeSymbol, Sides.Buy, 100, 11320).Result;

			// Assert
			Assert.NotNull(response);

			Thread.Sleep(10000000);

			var changed = fixConnector.ChangeLimitPrice(response, 2m).Result;
			changed = fixConnector.ChangeLimitPrice(response, 3m).Result && changed;
			changed = fixConnector.ChangeLimitPrice(response, 4m).Result && changed;
			changed = fixConnector.ChangeLimitPrice(response, 5m).Result && changed;
			changed = fixConnector.ChangeLimitPrice(response, 6m).Result && changed;

			// Assert
			Assert.IsTrue(changed);

			// Assert
			var canceled = fixConnector.CancelLimit(response).Result;

			// Assert
			Assert.IsTrue(canceled);
		}

		[Test]
		public void FeedTest()
		{
			// Arrange
			Spoof.FeedAccount.Connector.Subscribe(Spoof.FeedSymbol);

			// Assert
			Thread.Sleep(new TimeSpan(0, 0, 100));
		}

		[Test]
		public void LevelTwoTest()
		{
			// Arrange
			((IbIntegration.Connector)Spoof.FeedAccount.Connector).SubscribeLevel2(Spoof.FeedSymbol);

			// Assert
			Thread.Sleep(new TimeSpan(0, 0, 100));
		}
	}
}
