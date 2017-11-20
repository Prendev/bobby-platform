using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using log4net;
using QvaDev.Common.Services;
using QvaDev.CTraderIntegration.Dto;
using QvaDev.Data.Models;

namespace QvaDev.Orchestration.Services
{
    public interface IBalanceReportService
    {
        void Report(Monitor alphaMonitor, Monitor betaMonitor, DateTime from, DateTime to, string reportPath);
    }

    public class BalanceReportService : IBalanceReportService
    {
        public class BalanceReportData
        {
            public enum AccountGroups { Alpha, Beta }

            public AccountGroups AccountGroup { get; set; }
            public long Account { get; set; }
            public double Balance { get; set; }
            public double Pnl { get; set; }
            public string Currency { get; set; }
        }

        private readonly IExchangeRatesService _exchangeRatesService;
        private readonly ILog _log;

        public BalanceReportService(
            IExchangeRatesService exchangeRatesService,
            ILog log)
        {
            _log = log;
            _exchangeRatesService = exchangeRatesService;
        }

        public void Report(Monitor alphaMonitor, Monitor betaMonitor, DateTime from, DateTime to, string reportPath)
        {
            _log.Debug("Balance report is in progress...");
            CTraderIntegration.Connector.BalanceAccounts = new ConcurrentDictionary<string, Lazy<List<AccountData>>>();
            var balances = new List<BalanceReportData>();
            foreach (var monitoredAccount in alphaMonitor.MonitoredAccounts)
            {
                var connector = monitoredAccount.CTraderAccount?.Connector ??
                                monitoredAccount.MetaTraderAccount?.Connector;

                balances.Add(new BalanceReportData
                {
                    AccountGroup = BalanceReportData.AccountGroups.Alpha,
                    Account = monitoredAccount.CTraderAccount?.AccountNumber ?? monitoredAccount.MetaTraderAccount?.User ?? 0,
                    Balance = connector?.GetBalance() ?? 0,
                    Pnl = connector?.GetPnl(from, to) ?? 0,
                    Currency = connector?.GetCurrency() ?? ""
                });
            }
            foreach (var monitoredAccount in betaMonitor.MonitoredAccounts)
            {
                var connector = monitoredAccount.CTraderAccount?.Connector ??
                                monitoredAccount.MetaTraderAccount?.Connector;

                balances.Add(new BalanceReportData
                {
                    AccountGroup = BalanceReportData.AccountGroups.Beta,
                    Account = monitoredAccount.CTraderAccount?.AccountNumber ?? monitoredAccount.MetaTraderAccount?.User ?? 0,
                    Balance = connector?.GetBalance() ?? 0,
                    Pnl = connector?.GetPnl(from, to) ?? 0,
                    Currency = connector?.GetCurrency() ?? ""
                });
            }

            var rates = _exchangeRatesService.GetRates(ConfigurationManager.AppSettings["OpenExchangeRatesAppId"]);
            var sideA = balances.Where(b => b.AccountGroup == BalanceReportData.AccountGroups.Alpha).OrderBy(b => b.Account);
            var sideB = balances.Where(b => b.AccountGroup == BalanceReportData.AccountGroups.Beta).OrderBy(b => b.Account);
            var sideAlphaSum = sideA.Sum(b => ConvertToBaseCurrency(b, rates));
            var sideBetaSum = sideB.Sum(b => ConvertToBaseCurrency(b, rates));
            var templatePath = $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\Templates\BalanceReport.xlsx";

            using (var stream = new FileStream(reportPath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                var wb = new CustomWorkbook(templatePath);
                var sheet = wb.GetSheetAt(0);

                {
                    var row = sheet.GetRow(1) ?? sheet.CreateRow(1);
                    wb.CreateGeneralCell(row, 2, sideAlphaSum);
                    wb.CreateTextCellWithThichRigthBorder(row, 3, "USD");
                    wb.CreateGeneralCell(row, 6, sideBetaSum);
                    wb.CreateTextCellWithThichRigthBorder(row, 7, "USD");
                }

                for (var i = 0; i < sideA.Count(); i++)
                {
                    var b = sideA.ElementAt(i);
                    var row = sheet.GetRow(i + 3) ?? sheet.CreateRow(i + 3);
                    wb.CreateTextCell(row, 0, b.Account.ToString());
                    wb.CreateGeneralCell(row, 1, b.Balance);
                    wb.CreateGeneralCell(row, 2, b.Pnl);
                    wb.CreateTextCellWithThichRigthBorder(row, 3, b.Currency);
                }

                for (var i = 0; i < sideB.Count(); i++)
                {
                    var b = sideB.ElementAt(i);
                    var row = sheet.GetRow(i + 3) ?? sheet.CreateRow(i + 3);
                    wb.CreateTextCell(row, 4, b.Account.ToString());
                    wb.CreateGeneralCell(row, 5, b.Balance);
                    wb.CreateGeneralCell(row, 6, b.Pnl);
                    wb.CreateTextCellWithThichRigthBorder(row, 7, b.Currency);
                }

                wb.Write(stream);
            }
            _log.Debug("Balance report is READY!");
        }

        private double ConvertToBaseCurrency(BalanceReportData balance, Dictionary<string, decimal> rates)
        {
            var usdAmount = balance.Pnl / (double)rates[balance.Currency];
            var baseAmount = usdAmount * (double)rates["USD"];
            return baseAmount;
        }
    }
}
