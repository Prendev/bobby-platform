﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using QvaDev.Data;
using QvaDev.Data.Models;
using QvaDev.Orchestration.Services;

namespace QvaDev.Orchestration
{
    public interface IOrchestrator
    {
        int SelectedAlphaMonitorId { get; set; }
        int SelectedBetaMonitorId { get; set; }
        Task StartCopiers(DuplicatContext duplicatContext);
        void StopCopiers();
        Task StartMonitors(DuplicatContext duplicatContext, int alphaMonitorId, int betaMonitorId);
        void StopMonitors();
        Task Connect(DuplicatContext duplicatContext);
        Task Disconnect();
        Task BalanceReport(DateTime from);
    }

    public partial class Orchestrator : IOrchestrator
    {
        private SynchronizationContext _synchronizationContext;
        private DuplicatContext _duplicatContext;
        private readonly Func<SynchronizationContext> _synchronizationContextFactory;
        private readonly ILog _log;
        private readonly CTraderIntegration.IConnectorFactory _connectorFactory;
        private readonly IBalanceReportService _balanceReportService;

        public Orchestrator(
            Func<SynchronizationContext> synchronizationContextFactory,
            CTraderIntegration.IConnectorFactory connectorFactory,
            IBalanceReportService balanceReportService,
            ILog log)
        {
            _balanceReportService = balanceReportService;
            _synchronizationContextFactory = synchronizationContextFactory;
            _connectorFactory = connectorFactory;
            _log = log;
        }

        public Task Connect(DuplicatContext duplicatContext)
        {
            _duplicatContext = duplicatContext;
            _synchronizationContext = _synchronizationContext ?? _synchronizationContextFactory.Invoke();
            return Task.WhenAll(ConnectMtAccounts(), ConenctCtAccounts());
        }

        private Task ConnectMtAccounts()
        {
            var tasks = _duplicatContext.MetaTraderAccounts.AsEnumerable().Select(account =>
                Task.Factory.StartNew(() =>
                {
                    if (account.State == BaseAccountEntity.States.Connected) return;
                    var connector = account.Connector as Mt4Integration.Connector;
                    if (connector == null)
                    {
                        connector = new Mt4Integration.Connector(_log);
                        account.Connector = connector;
                    }
                    var connected = connector.Connect(new Mt4Integration.AccountInfo()
                    {
                        DbId = account.Id,
                        Description = account.Description,
                        User = (uint) account.User,
                        Password = account.Password,
                        Srv = account.MetaTraderPlatform.SrvFilePath
                    });
                    account.State = connected
                        ? BaseAccountEntity.States.Connected
                        : BaseAccountEntity.States.Error;
                    account.RaisePropertyChanged(_synchronizationContext, nameof(account.State));
                }));

            return Task.WhenAll(tasks);
        }

        private Task ConenctCtAccounts()
        {
            var tasks = _duplicatContext.CTraderAccounts.AsEnumerable().Select(account =>
                Task.Factory.StartNew(() =>
                {
                    if (account.State == BaseAccountEntity.States.Connected) return;
                    var connector = account.Connector as CTraderIntegration.Connector;
                    if (connector == null)
                    {
                        connector = (CTraderIntegration.Connector) _connectorFactory.Create(
                            new CTraderIntegration.PlatformInfo
                            {
                                Description = account.CTraderPlatform.Description,
                                AccountsApi = account.CTraderPlatform.AccountsApi,
                                ClientId = account.CTraderPlatform.ClientId,
                                TradingHost = account.CTraderPlatform.TradingHost,
                                Secret = account.CTraderPlatform.Secret,
                                Playground = account.CTraderPlatform.Playground
                            },
                            new CTraderIntegration.AccountInfo
                            {
                                DbId = account.Id,
                                Description = account.Description,
                                AccountNumber = account.AccountNumber,
                                AccessToken = account.AccessToken
                            });
                        account.Connector = connector;
                    }
                    var connected = connector.Connect();
                    account.State = connected
                        ? BaseAccountEntity.States.Connected
                        : BaseAccountEntity.States.Error;
                    account.RaisePropertyChanged(_synchronizationContext, nameof(account.State));
                }));

            return Task.WhenAll(tasks);
        }

        public Task Disconnect()
        {
            _areCopiersStarted = false;
            _areMonitorsStarted = false;
            return Task.WhenAll(DisconnectMtAccounts(), DisconnectCtAccounts());
        }

        private Task DisconnectMtAccounts()
        {
            var tasks = _duplicatContext.MetaTraderAccounts.AsEnumerable().Select(account =>
                Task.Factory.StartNew(() =>
                {
                    if (account.State == BaseAccountEntity.States.Disconnected) return;
                    account.Connector.Disconnect();
                    account.State = BaseAccountEntity.States.Disconnected;
                    account.RaisePropertyChanged(_synchronizationContext, nameof(account.State));
                }));

            return Task.WhenAll(tasks);
        }

        private Task DisconnectCtAccounts()
        {
            var tasks = _duplicatContext.CTraderAccounts.AsEnumerable().Select(account =>
                Task.Factory.StartNew(() =>
                {
                    if (account.State == BaseAccountEntity.States.Disconnected) return;
                    account.Connector.Disconnect();
                    account.State = BaseAccountEntity.States.Disconnected;
                    account.RaisePropertyChanged(_synchronizationContext, nameof(account.State));
                }));

            return Task.WhenAll(tasks);
        }
    }
}
