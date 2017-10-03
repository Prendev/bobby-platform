using QvaDev.Common.Integration;

namespace QvaDev.Configuration
{
    public class Config
    {
        public Config()
        {
            CommonConfigSection = new CommonConfigSection();
            MasterAccountsSection = new MasterAccountsSection();
            SlaveAccountsSection = new SlaveAccountsSection();
            ConnectorConfig = new ConnectorConfig();
        }

        public CommonConfigSection CommonConfigSection { get; set; }
        public MasterAccountsSection MasterAccountsSection { get; set; }
        public SlaveAccountsSection SlaveAccountsSection { get; set; }
        public ConnectorConfig ConnectorConfig { get; set; }
    }
}
