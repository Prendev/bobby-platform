using System.Collections.Generic;
using TradeSystem.Common.Integration;
using static nj4x.Metatrader.Broker;

namespace TradeSystem.Nj4xIntegration
{
	public class AccountInfo : BaseAccountInfo
    {
	    public class InstrumentConfig
	    {
		    public string Symbol { get; set; }
			/// <summary>
			/// "A value to multiply the sent quantities with and to divide the incoming ones."
			/// </summary>
			public decimal? Multiplier { get; set; }
		}

        public int User { get; set; }
        public string Password { get; set; }
        public string Srv { get; set; }
        public string BackupSrv { get; set; }

		public int? LocalPortForProxy { get; set; }
		public Dictionary<string, decimal> InstrumentConfigs { get; set; }
		public bool ProxyEnable { get; set; }
		public string ProxyHost { get; set; }
		public int ProxyPort { get; set; }
		public ProxyType ProxyType { get; set; }
		public string ProxyUser { get; set; }
		public string ProxyPassword { get; set; }
	}
}
