using System;
namespace TradeSystem.Orchestration.Services.Strategies
{
	public abstract class BaseStrategyService
	{
		protected int _throttlingInSec;

		public void SetThrottling(int throttlingInSec)
		{
			_throttlingInSec = throttlingInSec;
		}
	}
}
