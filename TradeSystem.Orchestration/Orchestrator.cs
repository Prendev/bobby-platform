using System;
using System.Threading;
using TradeSystem.Data;
using TradeSystem.Data.Models;
using TradeSystem.Orchestration.Services;

namespace TradeSystem.Orchestration
{
	public interface IOrchestrator
	{
		void Init(DuplicatContext duplicatContext);
		void CuttingTemplate(Quotation quotation);
	}

	public class Orchestrator : IOrchestrator
	{
		private SynchronizationContext _synchronizationContext;
		private DuplicatContext _duplicatContext;
		private readonly Func<SynchronizationContext> _synchronizationContextFactory;
		private readonly ISpreadsheetGenerator _spreadsheetGenerator;

		public Orchestrator(
			Func<SynchronizationContext> synchronizationContextFactory,
			ISpreadsheetGenerator spreadsheetGenerator)
		{
			_spreadsheetGenerator = spreadsheetGenerator;
			_synchronizationContextFactory = synchronizationContextFactory;
		}

		public void Init(DuplicatContext duplicatContext)
		{
			_duplicatContext = duplicatContext;
			_synchronizationContext = _synchronizationContext ?? _synchronizationContextFactory.Invoke();
		}

		public void CuttingTemplate(Quotation quotation) => _spreadsheetGenerator.CuttingTemplate(quotation);
	}
}
