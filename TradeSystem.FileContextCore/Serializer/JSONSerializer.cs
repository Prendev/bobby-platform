using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Metadata;
using Newtonsoft.Json.Linq;

namespace TradeSystem.FileContextCore.Serializer
{
	internal class JsonSerializer : ISerializer
    {
	    private readonly string[] _propertyKeys;
        private readonly Type[] _typeList;

        public JsonSerializer(IEntityType entityType)
        {
            _propertyKeys = entityType.GetProperties().Select(p => p.Name).ToArray();
            _typeList = entityType.GetProperties().Select(p => p.ClrType).ToArray();
        }

	    public Dictionary<TKey, object[]> Deserialize<TKey>(string list, Dictionary<TKey, object[]> newList, IReadOnlyList<IProperty> primaryKey)
	    {
		    if (string.IsNullOrWhiteSpace(list)) return newList;

		    foreach (var jToken in JArray.Parse(list))
		    {
			    var json = (JObject) jToken;

			    TKey key;

			    if (primaryKey.Count > 1)
			    {
				    var objKey = new object[primaryKey.Count];
				    for (var i = 0; i < objKey.Length; i++)
					    objKey[i] = json.Value<string>(primaryKey[i].Name).Deserialize(primaryKey[i].ClrType);

				    key = (TKey) (object) objKey;
			    }
			    else key = (TKey) json.Value<string>(primaryKey[0].Name).Deserialize(typeof(TKey));

				newList.Add(key, _propertyKeys.Select((t, i) => json.Value<string>(t).Deserialize(_typeList[i])).ToArray());
		    }

		    return newList;
		}

		public string Serialize<TKey>(Dictionary<TKey, object[]> list)
	    {
		    var array = new JArray();

		    foreach (var val in list)
		    {
			    var json = new JObject();

				for (var i = 0; i < _propertyKeys.Length; i++)
			    {
				    var property = new JProperty(_propertyKeys[i], val.Value[i].Serialize());
				    json.Add(property);
			    }

			    array.Add(json);
		    }

		    return array.ToString();
	    }
    }
}
