using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Sockets;
using QvaDev.Common.Attributes;

namespace QvaDev.Data.Models
{
	public partial class ProfileProxy : BaseEntity
	{
		[NotMapped] [InvisibleColumn] public TcpListener Listener { get => Get<TcpListener>(); set => Set(value); }
	}
}
