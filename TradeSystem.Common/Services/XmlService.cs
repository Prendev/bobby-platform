using System;
using System.IO;
using System.Xml.Serialization;

namespace TradeSystem.Common.Services
{
    public interface IXmlService
    {
        T DeserializeXmlFile<T>(string path);
        void Save<T>(T data, string path);
    }

    public class XmlService : IXmlService
    {
        public T DeserializeXmlFile<T>(string path)
        {
            var returnObject = default(T);
            if (string.IsNullOrEmpty(path)) return default(T);

            try
            {
                using (var xmlStream = new StreamReader(path))
                {
                    var serializer = new XmlSerializer(typeof(T));
                    returnObject = (T) serializer.Deserialize(xmlStream);
                }
            }
            catch (Exception e)
            {
	            Logger.Error("Failed to deserialize config file");
	            Logger.Info("DeserializeXmlFile", e);
            }
            return returnObject;
        }

        public void Save<T>(T data, string path)
        {
            if (string.IsNullOrEmpty(path)) return;

            try
            {
                using (var xmlStream = new StreamWriter(path))
                {
                    var serializer = new XmlSerializer(typeof(T));
                    serializer.Serialize(xmlStream, data);
                }
            }
            catch (Exception e)
            {
	            Logger.Error("Failed to serialize config file");
	            Logger.Info("SerializeXmlFile", e);
            }
        }
    }
}
