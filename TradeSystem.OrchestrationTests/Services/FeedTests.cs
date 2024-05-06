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
	public class FeedTests
	{
		private Account Account { get; set; }
		private string Symbol = "FUT|DTB|FDAX DEC 18";

		[SetUp]
		public void SetUp()
		{
			var connectorFactory = new ConnectorFactory(null, null, null, null);
			Account = new Account()
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

			connectorFactory.Create(Account).Wait();

			Assert.IsTrue(Account.Connector.IsConnected);
		}

		[TearDown]
		public void TearDown()
		{
			Account?.Connector?.Disconnect();
		}

		[Test]
		public void FeedTest()
		{
			// Arrange
			Account.Connector.Subscribe(Symbol);

			// Assert
			Thread.Sleep(new TimeSpan(0, 0, 100));
		}

		[Test]
		public void LevelTwoTest()
		{
			// Arrange
			((IbIntegration.Connector)Account.Connector).SubscribeLevel2(Symbol);

			// Assert
			Thread.Sleep(new TimeSpan(0, 0, 100));
		}
	}
}
