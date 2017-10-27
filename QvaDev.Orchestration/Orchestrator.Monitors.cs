using System;
using System.Linq;
using System.Threading.Tasks;
using QvaDev.Common.Integration;
using QvaDev.Data;
using QvaDev.Data.Models;

namespace QvaDev.Orchestration
{
    public partial class Orchestrator
    {
        private bool _areMonitorsStarted;

        public Task BalanceReport(DateTime from)
        {
            return Task.Factory.StartNew(() =>
            {
                _balanceReportService.Report(
                    _duplicatContext.Monitors.Local.FirstOrDefault(m => m.Id == SelectedAlphaMonitorId),
                    _duplicatContext.Monitors.Local.FirstOrDefault(m => m.Id == SelectedBetaMonitorId),
                    from);
            });
        }

        public Task StartMonitors(DuplicatContext duplicatContext, int alphaMonitorId, int betaMonitorId)
        {
            _areMonitorsStarted = true;
            return Connect(duplicatContext, alphaMonitorId, betaMonitorId).ContinueWith(prevTask =>
            {
                var mtTasks = _duplicatContext.MetaTraderAccounts.AsEnumerable().Select(account =>
                    Task.Factory.StartNew(() => MonitorAccount(account)));
                var ctTasks = _duplicatContext.CTraderAccounts.AsEnumerable().Select(account =>
                    Task.Factory.StartNew(() => MonitorAccount(account)));
                return Task.WhenAll(Task.WhenAll(mtTasks), Task.WhenAll(ctTasks));
            });
        }

        public void StopMonitors()
        {
            _areMonitorsStarted = false;
        }

        private void MonitorAccount(BaseAccountEntity account)
        {
            UpdateActualContracts(account);

            account.Connector.OnPosition -= Monitor_OnPosition;
            account.Connector.OnPosition += Monitor_OnPosition;
        }

        private void Monitor_OnPosition(object sender, PositionEventArgs e)
        {
            if (_areMonitorsStarted) return;
            BaseAccountEntity account = null;
            if (e.AccountType == AccountTypes.Ct)
                account = _duplicatContext.CTraderAccounts.Local.FirstOrDefault(a => a.Id == e.DbId);
            else if (e.AccountType == AccountTypes.Mt4)
                account = _duplicatContext.MetaTraderAccounts.Local.FirstOrDefault(a => a.Id == e.DbId);

            UpdateActualContracts(account);
        }

        private void UpdateActualContracts(BaseAccountEntity account)
        {
            if (account?.Connector == null) return;
            if (account.State != BaseAccountEntity.States.Connected) return;

            var monitored = account.MonitoredAccounts
                .FirstOrDefault(a => a.MonitorId == SelectedAlphaMonitorId || a.MonitorId == SelectedBetaMonitorId);
            if (monitored == null) return;

            var symbol = monitored.Symbol ?? monitored.Monitor.Symbol;
            monitored.ActualContracts = account.Connector.GetOpenContracts(symbol);
            monitored.RaisePropertyChanged(_synchronizationContext, nameof(monitored.ActualContracts));

            monitored.Monitor.ActualContracts = monitored.Monitor.MonitoredAccounts
                .Where(a => !a.IsMaster)
                .Sum(a => a.ActualContracts);
            monitored.Monitor.RaisePropertyChanged(_synchronizationContext, nameof(monitored.Monitor.ActualContracts));

            monitored.Monitor.ExpectedContracts = monitored.Monitor.MonitoredAccounts
                .Where(a => !a.IsMaster)
                .Sum(a => a.ExpectedContracts);
            monitored.Monitor.RaisePropertyChanged(_synchronizationContext, nameof(monitored.Monitor.ExpectedContracts));
        }
    }
}
