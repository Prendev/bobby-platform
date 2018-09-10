using System.Threading.Tasks;
using QvaDev.Data.Models;

namespace QvaDev.Data
{
	public interface IConnectorFactory
	{
		Task Create(Account account);
	}
}
