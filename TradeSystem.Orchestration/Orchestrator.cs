using System;
using System.Threading;
using System.Threading.Tasks;
using TradeSystem.Data;

namespace TradeSystem.Orchestration
{
	public interface IOrchestrator
	{
		void Init(DuplicatContext duplicatContext);
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

		public void Init(DuplicatContext duplicatContext)
		{
			_duplicatContext = duplicatContext;
			_synchronizationContext = _synchronizationContext ?? _synchronizationContextFactory.Invoke();
		}
	}
}
