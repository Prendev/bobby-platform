using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TradeSystem.Common.Integration;
using TradeSystem.Data;
using TradeSystem.Data.Models;
using TradingAPI.MT4Server;

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
					var user = long.Parse(record.User);
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
						mtAccount.Accounts.Add(new Account() {Profile = profile, Run = true});
				}

				duplicatContext.SaveChanges();
			}
		}

		public void SaveTheWeekend(DuplicatContext duplicatContext, DateTime from, DateTime to)
		{
			using (var streamWriter = new StreamWriter("saveTheWeekend.csv"))
			using (var csvWriter = new CsvHelper.CsvWriter(streamWriter))
			{
				var profile = duplicatContext.Profiles.First(p => p.Description == "*Import-Export");
				var accounts =
					profile.Accounts.Where(a => a.Run && a.MetaTraderAccountId.HasValue && a.ConnectionState == ConnectionStates.Connected);

				var symbols = LoadSymbols();

				foreach (var account in accounts)
				{
					var conn = (Mt4Integration.Connector)account.Connector;
					var orders = conn.QuoteClient.DownloadOrderHistory(from, to);
					if(orders?.Length == 0) continue;
					var us = orders.Where(o => symbols.Contains(o.Symbol));
					us = us.Where(o =>  o.Type == Op.Buy || o.Type == Op.Sell);

					foreach (var order in us)
					{
						var record = new
						{
							Holder = conn.QuoteClient.AccountName,
							Broker = conn.QuoteClient.Account.company,
							Account = account.MetaTraderAccount.User,
							ID = order.Ticket,
							order.OpenTime,
							Type = order.Type == Op.Buy ? "buy" : "sell",
							Size = order.Lots,
							order.Symbol,
							order.OpenPrice,
							Sl = order.StopLoss,
							Tp = order.TakeProfit,
							order.CloseTime,
							Price = order.ClosePrice,
							order.Commission,
							order.Swap,
							order.Profit
						};
						csvWriter.WriteRecord(record);
						csvWriter.NextRecord();
					}

				}
			}
		}

		private List<string> Symbols()
		{
			return new List<string>()
			{
				"#DJ30",
				"#DOW30",
				".US30",
				"[DJI30]",
				"DOWUSD",
				"IDX.US.30",
				"US.30..",
				"US30",
				"US30.c",
				"US30.CASH",
				"US30.i",
				"US30.p",
				"US30Cash",
				"US30-CFD",
				"US30s",
				"US30USD",
				"UsaInd",
				"WS30",
				"WS30.",
				"WS30.ccm",
				"WS30.lmx",
				"#NAS",
				"#NAS100",
				".US100",
				"US100",
				"US100.c",
				"US100s",
				"US100.CASH",
				"US100.i",
				"US100.p",
				"US.100",
				"US.100.",
				"US.100.."
			};
		}

		private List<string> LoadSymbols()
		{
			return File.ReadLines("symbols.csv").ToList();
		}
	}
}
