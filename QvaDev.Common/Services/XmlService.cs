using System;
using System.IO;
using System.Xml.Serialization;
using QvaDev.Common.Logging;

namespace QvaDev.Common.Services
{
    public interface IXmlService
    {
        T DeserializeXmlFile<T>(string path);
        void Save<T>(T data, string path);
    }

    public class XmlService : IXmlService
    {
        private readonly ILog _log;

        public XmlService(ILog log)
        {
            _log = log;
        }

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
                _log?.Error("Failed to deserialize config file");
                _log?.Info("DeserializeXmlFile", e);
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
                _log?.Error("Failed to serialize config file");
                _log?.Info("SerializeXmlFile", e);
            }
        }
    }
}
