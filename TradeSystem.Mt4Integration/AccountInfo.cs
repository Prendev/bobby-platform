using TradeSystem.Common.Integration;

namespace TradeSystem.Mt4Integration
{
	public class AccountInfo : BaseAccountInfo
    {
        public int User { get; set; }
        public string Password { get; set; }
        public string Srv { get; set; }

		public int? LocalPortForProxy { get; set; }
    }
}
