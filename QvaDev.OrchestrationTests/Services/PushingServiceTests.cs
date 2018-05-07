using System;
using Moq;
using NUnit.Framework;
using QvaDev.Common.Integration;
using QvaDev.Common.Services;
using QvaDev.Data.Models;
using QvaDev.FixTraderIntegration;
using QvaDev.Orchestration.Services.Strategies;
using FtConnector = QvaDev.FixTraderIntegration.IConnector;
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
		private Mock<FtConnector> FtConnectorMock { get; set; }

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
			FtConnectorMock = new Mock<FtConnector>();
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
		public void OpeningAlpha()
		{
			// Arrange
			var symbolInfo = new SymbolInfo() { SumContracts = 100 };
			FtConnectorMock
				.Setup(m => m.GetSymbolInfo(It.IsAny<string>()))
				.Returns(symbolInfo);
			FtConnectorMock
				.Setup(m => m.SendMarketOrderRequest(It.IsAny<string>(), It.IsAny<Sides>(), It.IsAny<double>(), It.IsAny<string>()))
				.Callback(() => symbolInfo.SumContracts += 3);

			AlphaConnectorMock
				.Setup(m => m.SendMarketOrderRequest(It.IsAny<string>(), It.IsAny<Sides>(), It.IsAny<double>(),
					It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns(new Position());

			var pushing = CreatePushing();

			// Act
			PushingService.OpeningAlpha(pushing);

			// Assert
			AlphaConnectorMock.Verify(m => m.SendMarketOrderRequest("alphaDummySymbol", Sides.Buy, 0.123, 0, null, 7, 123), Times.Once);
			FtConnectorMock.Verify(m => m.SendMarketOrderRequest("ftDummySymbol", Sides.Sell, 3, null), Times.Exactly(7));
			ThreadServiceMock.Verify(m => m.Sleep(It.IsAny<int>()), Times.Exactly(7));
			Assert.AreEqual(121, symbolInfo.SumContracts);

			// Arrange
			PushingService.OpeningAlpha(pushing);
			AlphaConnectorMock
				.Setup(m => m.SendMarketOrderRequest(It.IsAny<string>(), It.IsAny<Sides>(), It.IsAny<double>(),
					It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((Position) null);

			// Act
			var ex = Assert.Throws<Exception>(() => PushingService.OpeningAlpha(pushing));

			// Assert
			Assert.That(ex.Message, Is.EqualTo("PushingService.OpeningAlpha failed!!!"));
		}

		[Test]
		public void OpeningFinish()
		{
			// Arrange
			var symbolInfo = new SymbolInfo() { SumContracts = 100 };
			FtConnectorMock
				.Setup(m => m.GetSymbolInfo(It.IsAny<string>()))
				.Returns(symbolInfo);
			FtConnectorMock
				.Setup(m => m.SendMarketOrderRequest(It.IsAny<string>(), It.IsAny<Sides>(), It.IsAny<double>(), It.IsAny<string>()))
				.Callback(() => symbolInfo.SumContracts += 4);

			var pushing = CreatePushing();
			pushing.PushingDetail.SmallContractSize = 4;

			// Act
			PushingService.OpeningFinish(pushing);

			// Assert
			FtConnectorMock.Verify(m => m.SendMarketOrderRequest("ftDummySymbol", Sides.Sell, 4, null), Times.Exactly(8));
			FtConnectorMock.Verify(m => m.OrderMultipleCloseBy("ftDummySymbol"), Times.Once);
			ThreadServiceMock.Verify(m => m.Sleep(It.IsAny<int>()), Times.Exactly(8));
			Assert.AreEqual(132, symbolInfo.SumContracts);

			// Arrange
			pushing.IsHedgeClose = true;

			// Act
			PushingService.OpeningFinish(pushing);

			// Assert
			FtConnectorMock.Verify(m => m.OrderMultipleCloseBy("ftDummySymbol"), Times.Exactly(2));
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
		public void OpeningNoHedge()
		{
			// Arrange
			var pushing = CreatePushing();
			pushing.IsHedgeClose = false;
			pushing.PushingDetail.HedgeSignalContractLimit = 10;
			pushing.FirstCloseSide = Sides.Buy;

			// Act
			PushingService.OpeningHedge(pushing);

			// Assert
			ThreadServiceMock.VerifyNoOtherCalls();
			AlphaConnectorMock.VerifyNoOtherCalls();
			BetaConnectorMock.VerifyNoOtherCalls();
			FtConnectorMock.VerifyNoOtherCalls();

			// Arrange
			pushing.IsHedgeClose = true;
			var symbolInfo = new SymbolInfo() { SumContracts = 100 };
			FtConnectorMock
				.Setup(m => m.GetSymbolInfo(It.IsAny<string>()))
				.Returns(symbolInfo);
			FtConnectorMock
				.Setup(m => m.SendMarketOrderRequest(It.IsAny<string>(), It.IsAny<Sides>(), It.IsAny<double>(), It.IsAny<string>()))
				.Callback(() => symbolInfo.SumContracts += 3);

			// Act
			PushingService.OpeningHedge(pushing);

			// Assert
			FtConnectorMock.Verify(m => m.SendMarketOrderRequest("ftDummySymbol", Sides.Sell, 3, null), Times.Exactly(4));
			HedgeConnectorMock.Verify(m => m.SendMarketOrderRequest("hedgeDummySymbol", Sides.Buy, 0.234, 0, null, 7, 123), Times.Once);
			ThreadServiceMock.Verify(m => m.Sleep(It.IsAny<int>()), Times.Exactly(4));
		}

		[Test]
		public void ClosingSecond()
		{
			// Arrange
			var symbolInfo = new SymbolInfo() { SumContracts = 100 };
			FtConnectorMock
				.Setup(m => m.GetSymbolInfo(It.IsAny<string>()))
				.Returns(symbolInfo);
			FtConnectorMock
				.Setup(m => m.SendMarketOrderRequest(It.IsAny<string>(), It.IsAny<Sides>(), It.IsAny<double>(), It.IsAny<string>()))
				.Callback(() => symbolInfo.SumContracts += 3);

			var pushing = CreatePushing();
			pushing.IsHedgeClose = false;
			pushing.FirstCloseSide = Sides.Sell;

			// Act
			PushingService.ClosingSecond(pushing);

			// Assert
			AlphaConnectorMock.Verify(m => m.SendClosePositionRequests(pushing.AlphaPosition, null, 7, 123), Times.Once);
			BetaConnectorMock.Verify(m => m.SendClosePositionRequests(It.IsAny<Position>(), It.IsAny<double?>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
			AlphaConnectorMock.Verify(m => m.SendMarketOrderRequest(It.IsAny<string>(), It.IsAny<Sides>(), It.IsAny<double>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
			BetaConnectorMock.Verify(m => m.SendMarketOrderRequest(It.IsAny<string>(), It.IsAny<Sides>(), It.IsAny<double>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);

			FtConnectorMock.Verify(m => m.SendMarketOrderRequest("ftDummySymbol", Sides.Buy, 3, null), Times.Exactly(7));
			ThreadServiceMock.Verify(m => m.Sleep(It.IsAny<int>()), Times.Exactly(7));
			Assert.AreEqual(121, symbolInfo.SumContracts);

			// Arrange
			symbolInfo.SumContracts = 110;
			pushing.IsHedgeClose = true;
			pushing.FirstCloseSide = Sides.Buy;

			// Act
			PushingService.ClosingSecond(pushing);

			// Assert
			BetaConnectorMock.Verify(m => m.SendClosePositionRequests(pushing.BetaPosition, null, 7, 123), Times.Once);
			AlphaConnectorMock.Verify(m => m.SendClosePositionRequests(pushing.AlphaPosition, null, 7, 123), Times.Once);
			AlphaConnectorMock.Verify(m => m.SendMarketOrderRequest(It.IsAny<string>(), It.IsAny<Sides>(), It.IsAny<double>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
			BetaConnectorMock.Verify(m => m.SendMarketOrderRequest(It.IsAny<string>(), It.IsAny<Sides>(), It.IsAny<double>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);

			FtConnectorMock.Verify(m => m.SendMarketOrderRequest("ftDummySymbol", Sides.Sell, 3, null), Times.Exactly(4));
			ThreadServiceMock.Verify(m => m.Sleep(It.IsAny<int>()), Times.Exactly(11));
			Assert.AreEqual(122, symbolInfo.SumContracts);
		}

		[Test]
		public void ClosingFinish()
		{
			// Arrange
			var symbolInfo = new SymbolInfo() { SumContracts = 100 };
			FtConnectorMock
				.Setup(m => m.GetSymbolInfo(It.IsAny<string>()))
				.Returns(symbolInfo);
			FtConnectorMock
				.Setup(m => m.SendMarketOrderRequest(It.IsAny<string>(), It.IsAny<Sides>(), It.IsAny<double>(), It.IsAny<string>()))
				.Callback(() => symbolInfo.SumContracts += 4);
			FtConnectorMock
				.Setup(m => m.OrderMultipleCloseBy("ftDummySymbol"))
				.Callback(() => symbolInfo.SumContracts = 0);

			var pushing = CreatePushing();
			pushing.PushingDetail.SmallContractSize = 4;
			pushing.IsHedgeClose = true;
			pushing.FirstCloseSide = Sides.Sell;

			// Act
			PushingService.ClosingFinish(pushing);

			// Assert
			ThreadServiceMock.Verify(m => m.Sleep(It.IsAny<int>()), Times.Exactly(8));
			FtConnectorMock.Verify(m => m.SendMarketOrderRequest("ftDummySymbol", Sides.Buy, 4, null), Times.Exactly(8));
			BetaConnectorMock.Verify(m => m.SendClosePositionRequests(It.IsAny<Position>(), It.IsAny<double?>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
			AlphaConnectorMock.Verify(m => m.SendClosePositionRequests(It.IsAny<Position>(), It.IsAny<double?>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
			FtConnectorMock.Verify(m => m.OrderMultipleCloseBy(It.IsAny<string>()), Times.Never);
			Assert.AreEqual(132, symbolInfo.SumContracts);

			// Arrange
			pushing.IsHedgeClose = false;
			pushing.FirstCloseSide = Sides.Buy;

			// Act
			PushingService.ClosingFinish(pushing);

			// Assert
			ThreadServiceMock.Verify(m => m.Sleep(It.IsAny<int>()), Times.Exactly(16));
			FtConnectorMock.Verify(m => m.SendMarketOrderRequest("ftDummySymbol", Sides.Sell, 4, null), Times.Exactly(8));
			BetaConnectorMock.Verify(m => m.SendClosePositionRequests(It.IsAny<Position>(), It.IsAny<double?>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
			AlphaConnectorMock.Verify(m => m.SendClosePositionRequests(It.IsAny<Position>(), It.IsAny<double?>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
			FtConnectorMock.Verify(m => m.OrderMultipleCloseBy(It.IsAny<string>()), Times.Once);
			Assert.AreEqual(0, symbolInfo.SumContracts);
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
				BetaMaster = new MetaTraderAccount() { Connector = BetaConnectorMock.Object },
				AlphaMaster = new MetaTraderAccount() { Connector = AlphaConnectorMock.Object },
				FutureAccount = new FixTraderAccount() { Connector = FtConnectorMock.Object },
				HedgeAccount = new MetaTraderAccount() { Connector = HedgeConnectorMock.Object },
				PushingDetail = new PushingDetail()
				{
					AlphaLots = 0.123,
					BetaLots = 0.345,
					MaxRetryCount = 7,
					RetryPeriodInMilliseconds = 123,
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
