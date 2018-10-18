using System.ComponentModel.DataAnnotations;

namespace QvaDev.Data.Models
{
	public class Proxy : BaseDescriptionEntity
	{
		public enum ProxyTypes
		{
			Socks4,
			Socks5
		}

		public ProxyTypes Type { get; set; }
		[Required] public string Url { get; set; }
		public string User { get; set; }
		public string Password { get; set; }
	}
}
