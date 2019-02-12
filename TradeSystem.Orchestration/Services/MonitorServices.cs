using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using QvaDev.Common.Integration;
using QvaDev.Data;
using QvaDev.Data.Models;

namespace QvaDev.Orchestration.Services
{
    public interface IMonitorServices
    {
        Task Start(DuplicatContext duplicatContext, int alphaMonitorId, int betaMonitorId);
        void Stop();
    }

    public class MonitorServices : IMonitorServices
    {
        private bool _isStarted;
        private readonly ILog _log;
        private SynchronizationContext _synchronizationContext;
        private readonly Func<SynchronizationContext> _synchronizationContextFactory;
        private IEnumerable<CTraderAccount> _ctAccounts;
        private IEnumerable<MetaTraderAccount> _mtAccounts;

        public int SelectedAlphaMonitorId { get; set; }
        public int SelectedBetaMonitorId { get; set; }

        public MonitorServices(
            Func<SynchronizationContext> synchronizationContextFactory,
            ILog log)
        {
            _synchronizationContextFactory = synchronizationContextFactory;
            _log = log;
        }

        public Task Start(
            DuplicatContext duplicatContext,
            int alphaMonitorId, int betaMonitorId)
        {
            _synchronizationContext = _synchronizationContext ?? _synchronizationContextFactory.Invoke();
            _isStarted = true;
            SelectedAlphaMonitorId = alphaMonitorId;
            SelectedBetaMonitorId = betaMonitorId;
            _mtAccounts = duplicatContext.MetaTraderAccounts.AsEnumerable();
            _ctAccounts = duplicatContext.CTraderAccounts.AsEnumerable();
            var mtTasks = _mtAccounts.Select(account => Task.Factory.StartNew(() => MonitorAccount(account)));
            var ctTasks = _ctAccounts.Select(account => Task.Factory.StartNew(() => MonitorAccount(account)));
            return Task.WhenAll(Task.WhenAll(mtTasks), Task.WhenAll(ctTasks));
        }

        public void Stop()
        {
            _isStarted = false;
        }

        private void MonitorAccount(BaseAccountEntity account)
        {
            UpdateActualContracts(account);

            account.Connector.OnPosition -= Monitor_OnPosition;
            account.Connector.OnPosition += Monitor_OnPosition;
        }

        private void Monitor_OnPosition(object sender, PositionEventArgs e)
        {
            if (!_isStarted) return;
            BaseAccountEntity account = null;
            if (e.AccountType == AccountTypes.Ct)
                account = _ctAccounts?.FirstOrDefault(a => a.Id == e.DbId);
            else if (e.AccountType == AccountTypes.Mt4)
                account = _mtAccounts.FirstOrDefault(a => a.Id == e.DbId);

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
