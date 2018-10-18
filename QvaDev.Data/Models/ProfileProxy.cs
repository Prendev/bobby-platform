using QvaDev.Common.Attributes;

namespace QvaDev.Data.Models
{
	public partial class ProfileProxy : BaseEntity
	{
		[InvisibleColumn] public int ProfileId { get; set; }
		[InvisibleColumn] public Profile Profile { get; set; }

		public int ProxyId { get; set; }
		public Proxy Proxy { get; set; }

		[DisplayPriority(-1)] public bool Run { get; set; }

		public int LocalPort { get; set; }

		public string Destination { get; set; }
	}
}
