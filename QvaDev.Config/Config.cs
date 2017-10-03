using QvaDev.Common.Integration;

namespace QvaDev.Config
{
    public class Config
    {
        public CommonConfigSection CommonConfigSection { get; set; }
        public MasterAccountsSection MasterAccountsSection { get; set; }
        public SlaveAccountsSection SlaveAccountsSection { get; set; }
        public ConnectorConfig ConnectorConfig { get; set; }
    }
}
