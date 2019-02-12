using System;

namespace TradeSystem.Common.Services
{
	public interface IRndService
	{
		int Next(int minValue, int maxValue);
	}

	public class RndService : IRndService
	{
		private readonly Random _rnd;

		public RndService()
		{
			_rnd = new Random();
		}

		public int Next(int minValue, int maxValue)
		{
			return _rnd.Next(minValue, maxValue);
		}
	}
}
