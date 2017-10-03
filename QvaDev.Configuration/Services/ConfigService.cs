using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using log4net;
using QvaDev.Common.Configuration;

namespace QvaDev.Configuration.Services
{
    public interface IConfigService
    {
        Config Config { get; }
        void Save(Config config, string path);
        Config Load(string path);
    }

    public class ConfigService : IConfigService
    {
        private readonly ILog _log;
        public Config Config => GetConfig("Config.xml");

        public ConfigService(ILog log)
        {
            _log = log;
        }

        public Config Load(string path)
        {
            return GetConfig(path);
        }

        public void Save(Config config, string path)
        {
            if (string.IsNullOrEmpty(path)) return;

            try
            {
                using (var xmlStream = new StreamWriter(path))
                {
                    var serializer = new XmlSerializer(typeof(Config));
                    serializer.Serialize(xmlStream, config);
                }
            }
            catch (Exception e)
            {
                _log.Error("Failed to serialize config file");
                _log.Info("SerializeXmlFile", e);
            }
        }

        private Config GetConfig(string path)
        {
            var config = DeserializeXmlFile(path);
            foreach (var srv in Directory.GetFiles(".\\Mt4SrvFiles", "*.srv").Select(Path.GetFileNameWithoutExtension))
            {
                config.CommonConfigSection.Mt4Platforms.Add(new Mt4Platform()
                {
                    Description = srv,
                    SrvFilePath = $"Mt4SrvFiles\\{srv}.srv"
                });
            }

            foreach (var account in config.MasterAccountsSection.Mt4Accounts)
                account.Platform = config.CommonConfigSection.Mt4Platforms
                    .FirstOrDefault(p => p.Description == account.PlatformDescription);

            foreach (var account in config.SlaveAccountsSection.CTraderAccounts)
                account.Platform = config.CommonConfigSection.CTraderPlatforms
                    .FirstOrDefault(p => p.Description == account.PlatformDescription);

            return config;
        }

        private Config DeserializeXmlFile(string path)
        {
            var returnObject = default(Config);
            if (string.IsNullOrEmpty(path)) return default(Config);

            try
            {
                using (var xmlStream = new StreamReader(path))
                {
                    var serializer = new XmlSerializer(typeof(Config));
                    returnObject = (Config)serializer.Deserialize(xmlStream);
                }
            }
            catch (Exception e)
            {
                _log.Error("Failed to deserialize config file");
                _log.Info("DeserializeXmlFile", e);
            }
            return returnObject;
        }
    }
}
