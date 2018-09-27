using System;
using System.Collections.Generic;
using System.Globalization;

namespace QvaDev.FileContextCore.Serializer
{
    static class SerializerHelper
    {
        public static object Deserialize(this string input, Type type)
        {
            if (String.IsNullOrEmpty(input))
            {
                return type.GetDefaultValue();
			}

			if (type != typeof(string) && type.IsNullableType())
			{
				type = Nullable.GetUnderlyingType(type);
			}

			if (type == typeof(TimeSpan))
            {
                return TimeSpan.Parse(input, CultureInfo.InvariantCulture);
            }
            if (type == typeof(Guid))
            {
                return Guid.Parse(input);
            }

	        if (type?.IsEnum == true)
	        {
		        return Enum.Parse(type, input);
	        }

			if (type?.IsArray == true)
            {
                Type arrType = type.GetElementType();
                List<object> arr = new List<object>();

                foreach (string s in input.Split(','))
                {
                    arr.Add(s.Deserialize(arrType));
                }

                return arr.ToArray();
            }

	        // ReSharper disable once AssignNullToNotNullAttribute
            return Convert.ChangeType(input, type, CultureInfo.InvariantCulture);
        }

        public static string Serialize(this object input)
        {
            if (input != null)
            {
                if (input.GetType().IsArray)
                {
                    string result = "";

                    object[] arr = (object[])input;

                    for (int i = 0; i < arr.Length; i++)
                    {
                        result += arr[i].Serialize();

                        if (i + 1 < arr.Length)
                        {
                            result += ",";
                        }
                    }

                    return result;
                }

				return input is IFormattable formattable ? formattable.ToString(null, CultureInfo.InvariantCulture) : input.ToString();
			}

            return "";
        }
    }
}
