using System;
using System.Threading;
using System.Threading.Tasks;
using TradeSystem.Data;

namespace TradeSystem.Orchestration
{
	public interface IOrchestrator
	{
		Task Connect(DuplicatContext duplicatContext);
		Task Disconnect();
	}

	public class Orchestrator : IOrchestrator
	{
		private SynchronizationContext _synchronizationContext;
		private DuplicatContext _duplicatContext;
		private readonly Func<SynchronizationContext> _synchronizationContextFactory;

		public Orchestrator(
			Func<SynchronizationContext> synchronizationContextFactory)
		{
			_synchronizationContextFactory = synchronizationContextFactory;
		}

		public async Task Connect(DuplicatContext duplicatContext)
		{
			_duplicatContext = duplicatContext;
			_synchronizationContext = _synchronizationContext ?? _synchronizationContextFactory.Invoke();
		}

		public async Task Disconnect()
		{
			_duplicatContext.SaveChanges();
		}
	}
}
