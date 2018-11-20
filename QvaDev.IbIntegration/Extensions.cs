using IBApi;

namespace QvaDev.IbIntegration
{
	public static class Extensions
	{
		public static Contract ToContract(this string symbol)
		{
			if (string.IsNullOrWhiteSpace(symbol)) return null;
			var c = symbol.Split('|');
			if (c.Length != 4 && c.Length != 5) return null;

			var contract = new Contract()
			{
				Symbol = c[0],
				SecType = c[1],
				Currency = c[2],
				Exchange = c[3]
			};
			if (c.Length > 4) contract.PrimaryExch = c[4];

			return contract;
		}
	}
}
