using QvaDev.Common.Integration;

namespace QvaDev.Mt4Integration
{
    public class AccountInfo : BaseAccountInfo
    {
        public uint User { get; set; }
        public string Password { get; set; }
        public string Srv { get; set; }
    }
}
