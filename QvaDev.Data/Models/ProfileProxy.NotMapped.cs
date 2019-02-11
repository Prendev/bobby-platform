using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Sockets;
using TradeSystem.Common.Attributes;

namespace TradeSystem.Data.Models
{
	public partial class ProfileProxy
	{
		[NotMapped] [InvisibleColumn] public TcpListener Listener { get => Get<TcpListener>(); set => Set(value); }
	}
}
