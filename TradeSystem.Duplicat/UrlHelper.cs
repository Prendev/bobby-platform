using System;
using System.Text.RegularExpressions;

namespace TradeSystem.Duplicat
{
    public class UrlHelper
    {
        public static string Encode(string str)
        {
            var charClass = $"0-9a-zA-Z{Regex.Escape("-_.!~*'()")}";
            return Regex.Replace(str, $"[^{charClass}]", EncodeEvaluator).Replace(".", "%2E");
        }

        public static string EncodeEvaluator(Match match)
        {
            return match.Value == " " ? "+" : $"%{Convert.ToInt32(match.Value[0]):X2}";
        }

        public static string DecodeEvaluator(Match match)
        {
            return Convert.ToChar(int.Parse(match.Value.Substring(1), System.Globalization.NumberStyles.HexNumber)).ToString();
        }

        public static string Decode(string str)
        {
            return Regex.Replace(str.Replace('+', ' '), "%[0-9a-zA-Z][0-9a-zA-Z]", DecodeEvaluator);
        }
    }
}
