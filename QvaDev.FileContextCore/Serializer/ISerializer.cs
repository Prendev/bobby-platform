using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Metadata;

namespace QvaDev.FileContextCore.Serializer
{
    interface ISerializer
    {
        Dictionary<TKey, object[]> Deserialize<TKey>(string list, Dictionary<TKey, object[]> newList, IReadOnlyList<IProperty> primaryKey);
		string Serialize<TKey>(Dictionary<TKey, object[]> list);
    }
}
