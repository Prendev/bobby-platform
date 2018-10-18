using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Sockets;
using QvaDev.Common.Attributes;

namespace QvaDev.Data.Models
{
	public partial class ProfileProxy
	{
		[NotMapped] [InvisibleColumn] public TcpListener Listener { get => Get<TcpListener>(); set => Set(value); }
	}
}
