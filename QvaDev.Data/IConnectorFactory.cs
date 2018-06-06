using QvaDev.Data.Models;

namespace QvaDev.Data
{
	public interface IConnectorFactory
	{
		void Create(Account account);
	}
}
