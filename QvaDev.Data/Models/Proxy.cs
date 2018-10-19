using System.ComponentModel.DataAnnotations;

namespace QvaDev.Data.Models
{
	public class Proxy : BaseDescriptionEntity
	{
		public enum ProxyTypes
		{
			Http,
			Socks4,
			Socks5
		}

		public ProxyTypes Type { get; set; }
		[Required] public string Host { get; set; }
		public int Port { get; set; }
		public string User { get; set; }
		public string Password { get; set; }
	}
}
