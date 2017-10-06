using QvaDev.Common.Integration;

namespace QvaDev.Mt4Integration
{
    public class AccountInfo : BaseAccountInfo
    {
        public int User { get; set; }
        public string Password { get; set; }
        public string Srv { get; set; }
    }
}
