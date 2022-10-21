using System.Collections.Generic;
using System.Linq;

namespace TradeSystem.FileContextCore.Serializer
{
	public class SerializedCollection<T> : List<SerializedItem<T>>
	{
		public SerializedCollection(IEnumerable<T> coll)
			: base(coll == null ? new List<SerializedItem<T>>() : coll.Select(x => new SerializedItem<T>(x)))
		{
		}

		public T[] Values => this.Select(x => x.X).ToArray();
	}

	public class SerializedItem<T>
	{
		public SerializedItem(T x)
		{
			X = x;
		}

		public T X { get; set; }
	}
}
