using System;
using System.Globalization;
using System.IO;
using System.Linq;
using TradeSystem.Common.Integration;
using TradeSystem.Communication;
using TradeSystem.Communication.Mt5;
using TradeSystem.Data;
using TradeSystem.Data.Models;
using TradingAPI.MT4Server;
using OrderType = mtapi.mt5.OrderType;

namespace TradeSystem.Orchestration.Services
{
	public interface IMtAccountImportService
	{
		void Import(DuplicatContext duplicatContext);
		void SaveTheWeekend(DuplicatContext duplicatContext, DateTime from, DateTime to);
	}

	public class MtAccountImportService : IMtAccountImportService
	{
		private class Record
		{
			public string Server { get; set; }
			public string User { get; set; }
			public string Pass { get; set; }
		}

		public void Import(DuplicatContext duplicatContext)
		{
			using (var streamReader = new StreamReader("accounts.csv"))
			using (var csvReader = new CsvHelper.CsvReader(streamReader))
			{
				csvReader.Configuration.Delimiter = ",";
				csvReader.Configuration.HasHeaderRecord = false;
				var records = csvReader.GetRecords<Record>().ToList();

				var profile = duplicatContext.Profiles.FirstOrDefault(p => p.Description == "*Import-Export") ??
				              duplicatContext.Profiles.Add(new Profile() {Description = "*Import-Export"}).Entity;

				foreach (var srv in records.Select(r => r.Server).Distinct())
				{
					if (duplicatContext.MetaTraderPlatforms.Any(p => p.SrvFilePath == srv))
						continue;
					duplicatContext.MetaTraderPlatforms.Add(new MetaTraderPlatform()
					{
						Description = srv,
						SrvFilePath = srv
					});
				}

				duplicatContext.SaveChanges();

				foreach (var record in records)
				{
					var user = int.Parse(record.User);
					var mtAccount = duplicatContext.MetaTraderAccounts.FirstOrDefault(a => a.User == user);

					if (mtAccount == null)
					{
						var platform = duplicatContext.MetaTraderPlatforms.First(p => p.SrvFilePath == record.Server);
						mtAccount = duplicatContext.MetaTraderAccounts.Add(new MetaTraderAccount()
						{
							Description = record.User,
							User = user,
							Password = record.Pass,
							MetaTraderPlatform = platform
						}).Entity;
					}

					if (mtAccount.Accounts.All(a => a.Profile != profile))
						mtAccount.Accounts.AddSafe(new Account() {Profile = profile, Run = true});
				}

				duplicatContext.SaveChanges();
			}
		}

		public class SaveTheWeekendRecord
		{
			public string Holder { get; set; }
			public string Broker { get; set; }
			public long Account { get; set; }
			public long ID { get; set; }
			public DateTime OpenTime { get; set; }
			public string Type { get; set; }
			public double Size { get; set; }
			public string Symbol { get; set; }
			public double OpenPrice { get; set; }
			public double Sl { get; set; }
			public double Tp { get; set; }
			public DateTime CloseTime { get; set; }
			public double Price { get; set; }
			public double Commission { get; set; }
			public double Swap { get; set; }
			public double Profit { get; set; }
		}

		public void SaveTheWeekend(DuplicatContext duplicatContext, DateTime from, DateTime to)
		{
			try
			{
				using (var streamWriter = new StreamWriter("saveTheWeekend.csv"))
				using (var csvWriter = new CsvHelper.CsvWriter(streamWriter))
				{
					csvWriter.Configuration.CultureInfo = CultureInfo.InvariantCulture;
					var symbols = File.ReadLines("symbols.csv")
						.Select(s => s.ToLowerInvariant()).ToList();

					csvWriter.WriteHeader<SaveTheWeekendRecord>();
					csvWriter.NextRecord();

					var mt4Accounts = duplicatContext.Accounts
						.Where(a => a.Run && a.MetaTraderAccountId.HasValue &&
						            a.ConnectionState == ConnectionStates.Connected);
					foreach (var account in mt4Accounts)
					{
						try
						{
							var conn = (Mt4Integration.Connector) account.Connector;
							var orders = conn.QuoteClient.DownloadOrderHistory(from, to);
							if ((orders?.Length ?? 0) == 0) continue;
							var us = orders.Where(o => symbols.Contains(o.Symbol.ToLowerInvariant()));
							us = us.Where(o => o.Type == Op.Buy || o.Type == Op.Sell);

							foreach (var order in us)
							{
								var record = new SaveTheWeekendRecord()
								{
									Holder = conn.QuoteClient.AccountName,
									Broker = conn.QuoteClient.Account.company,
									Account = account.MetaTraderAccount.User,
									ID = order.Ticket,
									OpenTime = order.OpenTime,
									Type = order.Type == Op.Buy ? "buy" : "sell",
									Size = order.Lots,
									Symbol = order.Symbol,
									OpenPrice = order.OpenPrice,
									Sl = order.StopLoss,
									Tp = order.TakeProfit,
									CloseTime = order.CloseTime,
									Price = order.ClosePrice,
									Commission = order.Commission,
									Swap = order.Swap,
									Profit = order.Profit
								};
								csvWriter.WriteRecord(record);
								csvWriter.NextRecord();
							}
						}
						catch (Exception ex)
						{
							Logger.Error("MtAccountImportService.SaveTheWeekend error", ex);
						}
					}

					var mt5Accounts = duplicatContext.Accounts.Local.ToList()
						.Where(a => a.Run && a.FixApiAccountId.HasValue &&
						            a.ConnectionState == ConnectionStates.Connected &&
						            a.Connector is FixApiIntegration.Connector fixConnector &&
						            fixConnector.GeneralConnector is Mt5Connector);
					foreach (var account in mt5Accounts)
					{
						try
						{

							var conn = (Mt5Connector) ((FixApiIntegration.Connector) account.Connector)
								.GeneralConnector;
							var api = conn.Mt5Api;
							var orders = api.DownloadOrderHistory(from, to)?.Orders;
							if ((orders?.Capacity ?? 0) == 0) continue;
							var us = orders.Where(o => symbols.Contains(o.Symbol.ToLowerInvariant()));
							us = us.Where(o => o.OrderType == OrderType.Buy || o.OrderType == OrderType.Sell);

							foreach (var order in us)
							{
								var record = new SaveTheWeekendRecord()
								{
									Holder = api.Account.UserName,
									Broker = api.AccountCompanyName,
									Account = (long) api.Account.Login,
									ID = order.Ticket,
									OpenTime = order.OpenTime,
									Type = order.OrderType == OrderType.Buy ? "buy" : "sell",
									Size = order.Lots,
									Symbol = order.Symbol,
									OpenPrice = order.OpenPrice,
									Sl = order.StopLoss,
									Tp = order.TakeProfit,
									CloseTime = order.CloseTime,
									Price = order.ClosePrice,
									Commission = order.Commission,
									Swap = order.Swap,
									Profit = order.Profit
								};
								csvWriter.WriteRecord(record);
								csvWriter.NextRecord();
							}
						}
						catch (Exception ex)
						{
							Logger.Error("MtAccountImportService.SaveTheWeekend error", ex);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error("MtAccountImportService.SaveTheWeekend error", ex);
			}

			Logger.Info("MtAccountImportService.SaveTheWeekend done");
		}
	}
}
