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

		public string DestinationHost { get; set; }
		public int DestinationPort { get; set; }

		public override string ToString()
		{
			return $"{(Id == 0 ? "UNSAVED - " : "")}localhost:{LocalPort}";
		}
	}
}
