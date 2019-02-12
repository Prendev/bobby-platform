using System.Threading.Tasks;
using TradeSystem.Data.Models;

namespace TradeSystem.Data
{
	public interface IConnectorFactory
	{
		Task Create(Account account);
	}
}
