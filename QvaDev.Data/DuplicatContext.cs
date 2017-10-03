using System.Data.Entity;
using System.IO;
using System.Linq;
using QvaDev.Data.Models;

namespace QvaDev.Data
{
    public class DuplicatContext : DbContext
    {
        public DbSet<MetaTraderPlatform> MetaTraderPlatforms { get; set; }
        public DbSet<CTraderPlatform> CTraderPlatforms { get; set; }
        public DbSet<MetaTraderAccount> MetaTraderAccounts { get; set; }
        public DbSet<CTraderAccount> CTraderAccounts { get; set; }

        public void Init()
        {
            var exists = Database.Exists();
            if (!exists) Database.Create();

            foreach (var srv in Directory.GetFiles(".\\Mt4SrvFiles", "*.srv").Select(Path.GetFileNameWithoutExtension))
            {
                if (MetaTraderPlatforms.Any(p => p.Description == srv)) continue;
                MetaTraderPlatforms.Add(new MetaTraderPlatform()
                {
                    Description = srv,
                    SrvFilePath = $"Mt4SrvFiles\\{srv}.srv"
                });
            }

            if (exists) return;

            var cTraderProd = new CTraderPlatform()
            {
                Description = "cTrader Prod",
                AccountsApi = "https://api.spotware.com",
                TradingHost = "tradeapi.spotware.com",
                AccessToken = "802TOtADXELJqQ9gkiYfCLbqWWMqfbLWzzMPtg9PUa8",
                ClientId = "353_RTrdmK0Q01Czcaq1p19Y4RbJgngCn7Wc065c7QnFqHuUfr127S",
                Secret = "tQjMpGEbdvP68VXzm9sCQY3tuawrApSl5ZvmOGCAqov1eFT3zn",
                Playground = "https://connect.spotware.com/apps/353/playground"
            };
            CTraderPlatforms.Add(cTraderProd);

            SaveChanges();

            var mt4Platform = MetaTraderPlatforms.First(p => p.Description == "AuraFX-Demo");
            var cTraderPlatform = CTraderPlatforms.First(p => p.Description == "cTrader Prod");

            MetaTraderAccounts.Add(new MetaTraderAccount()
            {
                Description = "Aura1",
                Username = "8801562",
                Password = "k0agjjf",
                MetaTraderPlatform = mt4Platform
            });
            MetaTraderAccounts.Add(new MetaTraderAccount()
            {
                Description = "Aura2",
                Username = "8801567",
                Password = "4kfmvmo",
                MetaTraderPlatform = mt4Platform
            });

            CTraderAccounts.Add(new CTraderAccount()
            {
                Description = "cTrader1",
                AccountNumber = 3174636,
                CTraderPlatform = cTraderPlatform
            });
            CTraderAccounts.Add(new CTraderAccount()
            {
                Description = "cTrader2",
                AccountNumber = 3174645,
                CTraderPlatform = cTraderPlatform
            });
            CTraderAccounts.Add(new CTraderAccount()
            {
                Description = "cTrader3",
                AccountNumber = 3175387,
                CTraderPlatform = cTraderPlatform
            });
            CTraderAccounts.Add(new CTraderAccount()
            {
                Description = "cTrader4",
                AccountNumber = 3175388,
                CTraderPlatform = cTraderPlatform
            });

            SaveChanges();
        }
    }
}
