using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Metadata;
using Newtonsoft.Json.Linq;

namespace QvaDev.FileContextCore.Serializer
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

	    public Dictionary<TKey, object[]> Deserialize<TKey>(string list, Dictionary<TKey, object[]> newList)
	    {
		    if (string.IsNullOrWhiteSpace(list)) return newList;

		    foreach (var jToken in JArray.Parse(list))
		    {
			    var json = (JObject) jToken;
			    var key = (TKey) json.Value<string>("Id").Deserialize(typeof(TKey));

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
