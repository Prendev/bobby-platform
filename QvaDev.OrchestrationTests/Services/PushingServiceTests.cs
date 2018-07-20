using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using QvaDev.Common.Integration;
using QvaDev.Common.Services;
using QvaDev.Data.Models;
using QvaDev.Orchestration.Services.Strategies;
using FtConnector = QvaDev.Common.Integration.IFixConnector;
using MtConnector = QvaDev.Mt4Integration.IConnector;

namespace QvaDev.OrchestrationTests.Services
{
	[TestFixture]
	public class PushingServiceTests
	{
		private PushingService PushingService { get; set; }
		private Mock<IRndService> RndServiceMock { get; set; }
		private Mock<IThreadService> ThreadServiceMock { get; set; }
		private Mock<MtConnector> HedgeConnectorMock { get; set; }
		private Mock<MtConnector> BetaConnectorMock { get; set; }
		private Mock<MtConnector> AlphaConnectorMock { get; set; }
		private Mock<IFixConnector> FixConnectorMock { get; set; }

		[SetUp]
		public void SetUp()
		{
			RndServiceMock = new Mock<IRndService>();
			RndServiceMock.Setup(m => m.Next(It.IsAny<int>(), It.IsAny<int>())).Returns(100);
			ThreadServiceMock = new Mock<IThreadService>();
			PushingService = new PushingService(RndServiceMock.Object, ThreadServiceMock.Object);
			HedgeConnectorMock = new Mock<MtConnector>();
			BetaConnectorMock = new Mock<MtConnector>();
			AlphaConnectorMock = new Mock<MtConnector>();
			FixConnectorMock = new Mock<IFixConnector>();
		}

		[Test]
		public void OpeningBetaNoPosition()
		{
			// Arrange
			BetaConnectorMock
				.Setup(x => x.SendMarketOrderRequest(It.IsAny<string>(), It.IsAny<Sides>(), It.IsAny<double>(),
					It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((Position) null);
			var pushing = CreatePushing();

			// Act
			var ex = Assert.Throws<Exception>(() => PushingService.OpeningBeta(pushing));

			// Assert
			Assert.That(ex.Message, Is.EqualTo("PushingService.OpeningBeta failed!!!"));
		}

		[Test]
		public void OpeningBeta()
		{
			// Arrange
			BetaConnectorMock
				.Setup(m => m.SendMarketOrderRequest(It.IsAny<string>(), It.IsAny<Sides>(), It.IsAny<double>(),
					It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns(new Position());
			var pushing = CreatePushing();

			// Act
			PushingService.OpeningBeta(pushing);

			// Assert
			BetaConnectorMock.Verify(m => m.SendMarketOrderRequest("betaDummySymbol", Sides.Sell, 0.345, 0, null, 7, 123), Times.Once);
			ThreadServiceMock.Verify(m => m.Sleep(4321), Times.Once);
		}

		[Test]
		public async Task OpeningAlpha()
		{
			// Arrange
			FixConnectorMock
				.Setup(m => m.SendMarketOrderRequest(It.IsAny<string>(), It.IsAny<Sides>(), It.IsAny<decimal>(), It.IsAny<string>()))
				.Returns<string, Sides, decimal, string>((sym, s, q, c) => Task.FromResult(new OrderResponse() { FilledQuantity = q }));

			AlphaConnectorMock
				.Setup(m => m.SendMarketOrderRequest(It.IsAny<string>(), It.IsAny<Sides>(), It.IsAny<double>(),
					It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns(new Position());

			var pushing = CreatePushing();

			// Act
			await PushingService.OpeningAlpha(pushing);

			// Assert
			AlphaConnectorMock.Verify(m => m.SendMarketOrderRequest("alphaDummySymbol", Sides.Buy, 0.123, 0, null, 7, 123), Times.Once);
			FixConnectorMock.Verify(m => m.SendMarketOrderRequest("ftDummySymbol", Sides.Sell, 3, null), Times.Exactly(6));
			FixConnectorMock.Verify(m => m.SendMarketOrderRequest("ftDummySymbol", Sides.Sell, 2, null), Times.Exactly(1));
			FixConnectorMock.Verify(m => m.SendMarketOrderRequest(It.IsAny<string>(), Sides.Buy, It.IsAny<decimal>(), It.IsAny<string>()), Times.Never);
			ThreadServiceMock.Verify(m => m.Sleep(It.IsAny<int>()), Times.Exactly(7));

			// Arrange
			await PushingService.OpeningAlpha(pushing);
			AlphaConnectorMock
				.Setup(m => m.SendMarketOrderRequest(It.IsAny<string>(), It.IsAny<Sides>(), It.IsAny<double>(),
					It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((Position) null);

			// Act
			var ex = Assert.ThrowsAsync<Exception>(async() => await PushingService.OpeningAlpha(pushing));

			// Assert
			Assert.That(ex.Message, Is.EqualTo("PushingService.OpeningAlpha failed!!!"));
		}

		[Test]
		public async Task OpeningFinishNoHedge()
		{
			// Arrange
			FixConnectorMock
				.Setup(m => m.SendMarketOrderRequest(It.IsAny<string>(), It.IsAny<Sides>(), It.IsAny<decimal>(), It.IsAny<string>()))
				.Returns<string, Sides, decimal, string>((sym, s, q, c) => Task.FromResult(new OrderResponse() { FilledQuantity = q }));

			var pushing = CreatePushing();
			pushing.PushingDetail.SmallContractSize = 4;
			pushing.PushingDetail.OpenedFutures = 100;
			pushing.IsHedgeClose = false;

			// Act
			await PushingService.OpeningFinish(pushing);

			// Assert
			FixConnectorMock.Verify(m => m.SendMarketOrderRequest("ftDummySymbol", Sides.Sell, 4, null), Times.Exactly(7));
			FixConnectorMock.Verify(m => m.SendMarketOrderRequest("ftDummySymbol", Sides.Sell, 2, null), Times.Exactly(1));
			FixConnectorMock.Verify(m => m.SendMarketOrderRequest("ftDummySymbol", Sides.Buy, 130, null), Times.Once);
			FixConnectorMock.Verify(m => m.OrderMultipleCloseBy(It.IsAny<string>()), Times.Never);
			ThreadServiceMock.Verify(m => m.Sleep(It.IsAny<int>()), Times.Exactly(8));
		}

		[Test]
		public async Task OpeningFinishHedge()
		{
			// Arrange
			FixConnectorMock
				.Setup(m => m.SendMarketOrderRequest(It.IsAny<string>(), It.IsAny<Sides>(), It.IsAny<decimal>(), It.IsAny<string>()))
				.Returns<string, Sides, decimal, string>((sym, s, q, c) => Task.FromResult(new OrderResponse() { FilledQuantity = q }));

			var pushing = CreatePushing();
			pushing.PushingDetail.SmallContractSize = 4;
			pushing.PushingDetail.OpenedFutures = 100;
			pushing.IsHedgeClose = true;
			pushing.BetaOpenSide = Sides.Buy;

			// Act
			await PushingService.OpeningFinish(pushing);

			// Assert
			FixConnectorMock.Verify(m => m.SendMarketOrderRequest("ftDummySymbol", Sides.Buy, 4, null), Times.Exactly(7));
			FixConnectorMock.Verify(m => m.SendMarketOrderRequest("ftDummySymbol", Sides.Buy, 2, null), Times.Exactly(1));
			FixConnectorMock.Verify(m => m.SendMarketOrderRequest("ftDummySymbol", Sides.Sell, 130, null), Times.Once);
			FixConnectorMock.Verify(m => m.OrderMultipleCloseBy(It.IsAny<string>()), Times.Never);
			ThreadServiceMock.Verify(m => m.Sleep(It.IsAny<int>()), Times.Exactly(8));
		}

		[Test]
		public void ClosingFirstBeta()
		{
			// Arrange
			var pushing = CreatePushing();

			pushing.FirstCloseSide = Sides.Sell;

			// Act
			PushingService.ClosingFirst(pushing);

			// Assert
			ThreadServiceMock.Verify(m => m.Sleep(4321), Times.Once);
			BetaConnectorMock.Verify(m => m.SendClosePositionRequests(pushing.BetaPosition, null, 7, 123), Times.Once);
			AlphaConnectorMock.Verify(m => m.SendClosePositionRequests(It.IsAny<Position>(), It.IsAny<double?>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
		}

		[Test]
		public void ClosingFirstAlpha()
		{
			// Arrange
			var pushing = CreatePushing();

			pushing.FirstCloseSide = Sides.Buy;

			// Act
			PushingService.ClosingFirst(pushing);

			// Assert
			ThreadServiceMock.Verify(m => m.Sleep(4321), Times.Once);
			AlphaConnectorMock.Verify(m => m.SendClosePositionRequests(pushing.AlphaPosition, null, 7, 123), Times.Once);
			BetaConnectorMock.Verify(m => m.SendClosePositionRequests(It.IsAny<Position>(), It.IsAny<double?>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
		}

		[Test]
		public async Task OpeningHedge()
		{
			// Arrange
			FixConnectorMock
				.Setup(m => m.SendMarketOrderRequest(It.IsAny<string>(), It.IsAny<Sides>(), It.IsAny<decimal>(), It.IsAny<string>()))
				.Returns<string, Sides, decimal, string>((sym, s, q, c) => Task.FromResult(new OrderResponse() { FilledQuantity = q }));

			var pushing = CreatePushing();
			pushing.IsHedgeClose = false;
			pushing.PushingDetail.HedgeSignalContractLimit = 10;
			pushing.FirstCloseSide = Sides.Buy;

			// Act
			await PushingService.OpeningHedge(pushing);

			// Assert
			ThreadServiceMock.VerifyNoOtherCalls();
			AlphaConnectorMock.VerifyNoOtherCalls();
			BetaConnectorMock.VerifyNoOtherCalls();
			FixConnectorMock.VerifyNoOtherCalls();

			// Arrange
			pushing.IsHedgeClose = true;

			// Act
			await PushingService.OpeningHedge(pushing);

			// Assert
			FixConnectorMock.Verify(m => m.SendMarketOrderRequest("ftDummySymbol", Sides.Sell, 3, null), Times.Exactly(3));
			FixConnectorMock.Verify(m => m.SendMarketOrderRequest("ftDummySymbol", Sides.Sell, 1, null), Times.Exactly(1));
			FixConnectorMock.Verify(m => m.SendMarketOrderRequest(It.IsAny<string>(), Sides.Buy, It.IsAny<decimal>(), It.IsAny<string>()), Times.Never);
			HedgeConnectorMock.Verify(m => m.SendMarketOrderRequest("hedgeDummySymbol", Sides.Buy, 0.234, 0, null, 7, 123), Times.Once);
			ThreadServiceMock.Verify(m => m.Sleep(It.IsAny<int>()), Times.Exactly(4));
		}

		[Test]
		public async Task ClosingSecondNoHedge()
		{
			// Arrange
			FixConnectorMock
				.Setup(m => m.SendMarketOrderRequest(It.IsAny<string>(), It.IsAny<Sides>(), It.IsAny<decimal>(), It.IsAny<string>()))
				.Returns<string, Sides, decimal, string>((sym, s, q, c) => Task.FromResult(new OrderResponse() { FilledQuantity = q }));

			var pushing = CreatePushing();
			pushing.IsHedgeClose = false;
			pushing.FirstCloseSide = Sides.Sell;

			// Act
			await PushingService.ClosingSecond(pushing);

			// Assert
			AlphaConnectorMock.Verify(m => m.SendClosePositionRequests(pushing.AlphaPosition, null, 7, 123), Times.Once);
			BetaConnectorMock.Verify(m => m.SendClosePositionRequests(It.IsAny<Position>(), It.IsAny<double?>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
			AlphaConnectorMock.Verify(m => m.SendMarketOrderRequest(It.IsAny<string>(), It.IsAny<Sides>(), It.IsAny<double>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
			BetaConnectorMock.Verify(m => m.SendMarketOrderRequest(It.IsAny<string>(), It.IsAny<Sides>(), It.IsAny<double>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);

			FixConnectorMock.Verify(m => m.SendMarketOrderRequest("ftDummySymbol", Sides.Buy, 3, null), Times.Exactly(6));
			FixConnectorMock.Verify(m => m.SendMarketOrderRequest("ftDummySymbol", Sides.Buy, 2, null), Times.Exactly(1));
			FixConnectorMock.Verify(m => m.SendMarketOrderRequest(It.IsAny<string>(), Sides.Sell, It.IsAny<decimal>(), It.IsAny<string>()), Times.Never);
			FixConnectorMock.Verify(m => m.OrderMultipleCloseBy(It.IsAny<string>()), Times.Never);
			ThreadServiceMock.Verify(m => m.Sleep(It.IsAny<int>()), Times.Exactly(7));
		}

		[Test]
		public async Task ClosingSecondHedge()
		{
			// Arrange
			FixConnectorMock
				.Setup(m => m.SendMarketOrderRequest(It.IsAny<string>(), It.IsAny<Sides>(), It.IsAny<decimal>(), It.IsAny<string>()))
				.Returns<string, Sides, decimal, string>((sym, s, q, c) => Task.FromResult(new OrderResponse() { FilledQuantity = q }));

			var pushing = CreatePushing();
			pushing.IsHedgeClose = true;
			pushing.FirstCloseSide = Sides.Buy;

			// Act
			await PushingService.ClosingSecond(pushing);

			// Assert
			BetaConnectorMock.Verify(m => m.SendClosePositionRequests(pushing.BetaPosition, null, 7, 123), Times.Once);
			AlphaConnectorMock.Verify(m => m.SendClosePositionRequests(It.IsAny<Position>(), It.IsAny<double?>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
			AlphaConnectorMock.Verify(m => m.SendMarketOrderRequest(It.IsAny<string>(), It.IsAny<Sides>(), It.IsAny<double>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
			BetaConnectorMock.Verify(m => m.SendMarketOrderRequest(It.IsAny<string>(), It.IsAny<Sides>(), It.IsAny<double>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);

			FixConnectorMock.Verify(m => m.SendMarketOrderRequest("ftDummySymbol", Sides.Sell, 3, null), Times.Exactly(3));
			FixConnectorMock.Verify(m => m.SendMarketOrderRequest("ftDummySymbol", Sides.Sell, 1, null), Times.Exactly(1));
			FixConnectorMock.Verify(m => m.SendMarketOrderRequest(It.IsAny<string>(), Sides.Buy, It.IsAny<decimal>(), It.IsAny<string>()), Times.Never);
			FixConnectorMock.Verify(m => m.OrderMultipleCloseBy(It.IsAny<string>()), Times.Never);
			ThreadServiceMock.Verify(m => m.Sleep(It.IsAny<int>()), Times.Exactly(4));
		}

		[Test]
		public async Task ClosingFinishNoHedge()
		{
			// Arrange
			FixConnectorMock
				.Setup(m => m.SendMarketOrderRequest(It.IsAny<string>(), It.IsAny<Sides>(), It.IsAny<decimal>(), It.IsAny<string>()))
				.Returns<string, Sides, decimal, string>((sym, s, q, c) => Task.FromResult(new OrderResponse() {FilledQuantity = q}));

			var pushing = CreatePushing();
			pushing.PushingDetail.SmallContractSize = 4;
			pushing.PushingDetail.OpenedFutures = 100;
			pushing.IsHedgeClose = false;
			pushing.FirstCloseSide = Sides.Sell;

			// Act
			await PushingService.ClosingFinish(pushing);

			// Assert
			ThreadServiceMock.Verify(m => m.Sleep(It.IsAny<int>()), Times.Exactly(8));
			FixConnectorMock.Verify(m => m.SendMarketOrderRequest("ftDummySymbol", Sides.Buy, 4, null), Times.Exactly(7));
			FixConnectorMock.Verify(m => m.SendMarketOrderRequest("ftDummySymbol", Sides.Buy, 2, null), Times.Exactly(1));
			FixConnectorMock.Verify(m => m.SendMarketOrderRequest("ftDummySymbol", Sides.Sell, 130, null), Times.Exactly(1));
			BetaConnectorMock.Verify(m => m.SendClosePositionRequests(It.IsAny<Position>(), It.IsAny<double?>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
			AlphaConnectorMock.Verify(m => m.SendClosePositionRequests(It.IsAny<Position>(), It.IsAny<double?>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
			FixConnectorMock.Verify(m => m.OrderMultipleCloseBy(It.IsAny<string>()), Times.Never);
		}

		[Test]
		public async Task ClosingFinishHedge()
		{
			// Arrange
			FixConnectorMock
				.Setup(m => m.SendMarketOrderRequest(It.IsAny<string>(), It.IsAny<Sides>(), It.IsAny<decimal>(), It.IsAny<string>()))
				.Returns<string, Sides, decimal, string>((sym, s, q, c) => Task.FromResult(new OrderResponse() { FilledQuantity = q }));

			var pushing = CreatePushing();
			pushing.PushingDetail.SmallContractSize = 4;
			pushing.PushingDetail.OpenedFutures = 100;
			pushing.IsHedgeClose = true;
			pushing.FirstCloseSide = Sides.Buy;

			// Act
			await PushingService.ClosingFinish(pushing);

			// Assert
			ThreadServiceMock.Verify(m => m.Sleep(It.IsAny<int>()), Times.Exactly(8));
			FixConnectorMock.Verify(m => m.SendMarketOrderRequest("ftDummySymbol", Sides.Sell, 4, null), Times.Exactly(7));
			FixConnectorMock.Verify(m => m.SendMarketOrderRequest("ftDummySymbol", Sides.Sell, 2, null), Times.Exactly(1));
			FixConnectorMock.Verify(m => m.SendMarketOrderRequest("ftDummySymbol", Sides.Buy, It.IsAny<decimal>(), null), Times.Never);
			BetaConnectorMock.Verify(m => m.SendClosePositionRequests(It.IsAny<Position>(), It.IsAny<double?>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
			AlphaConnectorMock.Verify(m => m.SendClosePositionRequests(It.IsAny<Position>(), It.IsAny<double?>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
			FixConnectorMock.Verify(m => m.OrderMultipleCloseBy(It.IsAny<string>()), Times.Never);
		}

		private Pushing CreatePushing()
		{
			return new Pushing()
			{
				IsHedgeClose = false,
				FutureSymbol = "ftDummySymbol",
				BetaSymbol = "betaDummySymbol",
				AlphaSymbol = "alphaDummySymbol",
				HedgeSymbol = "hedgeDummySymbol",
				BetaOpenSide = Sides.Sell,
				BetaMaster = new Account() { Connector = BetaConnectorMock.Object },
				AlphaMaster = new Account() { Connector = AlphaConnectorMock.Object },
				FutureAccount = new Account() { Connector = FixConnectorMock.Object },
				HedgeAccount = new Account() { Connector = HedgeConnectorMock.Object },
				PushingDetail = new PushingDetail()
				{
					AlphaLots = 0.123,
					BetaLots = 0.345,
					MaxRetryCount = 7,
					RetryPeriodInMs = 123,
					FutureOpenDelayInMs = 4321,
					FullContractSize = 50,
					HedgeSignalContractLimit = 10,
					MasterSignalContractLimit = 30,
					MaxIntervalInMs = 50,
					SmallContractSize = 3,
					HedgeLots = 0.234
				}
			};
		}
	}
}
