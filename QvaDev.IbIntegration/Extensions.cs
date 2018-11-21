using IBApi;

namespace QvaDev.IbIntegration
{
	public static class Extensions
	{
		public static Contract ToContract(this string symbol)
		{
			if (string.IsNullOrWhiteSpace(symbol)) return null;
			var c = symbol.Split('|');
			if (c.Length != 3) return null;

			var contract = new Contract()
			{
				SecType = c[0],
				Exchange = c[1],
				LocalSymbol = c[2],
			};

			return contract;
		}
	}
}
