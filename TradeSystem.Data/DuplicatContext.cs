using System;
using System.Configuration;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using TradeSystem.Data.Models;
using TradeSystem.FileContextCore.Extensions;

namespace TradeSystem.Data
{
	public class DuplicatContext : DbContext
	{
		private readonly string _connectionString;
		private readonly string _providerName;
		private readonly string _baseDirectory;

		public DuplicatContext()
		{
			var connectionString = ConfigurationManager.ConnectionStrings["DuplicatContext"];

			_connectionString = connectionString.ConnectionString;

			var dd = AppDomain.CurrentDomain.GetData("DataDirectory")?.ToString();
			if (!string.IsNullOrWhiteSpace(dd) && _connectionString.Contains("|DataDirectory|"))
				_connectionString = _connectionString.Replace("|DataDirectory|", dd);

			_providerName = connectionString.ProviderName.ToLowerInvariant();
			_baseDirectory = AppContext.BaseDirectory;
		}

		public DuplicatContext(string connectionString, string providerName, string baseDirectory)
		{
			_providerName = providerName;
			_connectionString = connectionString;
			_baseDirectory = baseDirectory;
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if (_providerName == "filecontext")
				optionsBuilder.UseFileContext(_connectionString, _baseDirectory);
			else if (_providerName == "mssql")
				optionsBuilder.UseSqlServer(_connectionString);
			else if (_providerName == "sqlite")
				optionsBuilder.UseSqlite(_connectionString);
			else throw new ArgumentException("ProviderName");
		}

		public DbSet<MetaTraderPlatform> MetaTraderPlatforms { get; set; }
		public DbSet<CTraderPlatform> CTraderPlatforms { get; set; }
		public DbSet<MetaTraderAccount> MetaTraderAccounts { get; set; }
		public DbSet<MetaTraderInstrumentConfig> MetaTraderInstrumentConfigs { get; set; }
		public DbSet<MetaTraderPosition> MetaTraderPositions { get; set; }
		public DbSet<CTraderAccount> CTraderAccounts { get; set; }
		public DbSet<FixApiAccount> FixApiAccounts { get; set; }
		public DbSet<IlyaFastFeedAccount> IlyaFastFeedAccounts { get; set; }
		public DbSet<CqgClientApiAccount> CqgClientApiAccounts { get; set; }
		public DbSet<IbAccount> IbAccounts { get; set; }
		public DbSet<BacktesterAccount> BacktesterAccounts { get; set; }
		public DbSet<BacktesterInstrumentConfig> BacktesterInstrumentConfigs { get; set; }

		public DbSet<Profile> Profiles { get; set; }
		public DbSet<CustomGroup> CustomGroups { get; set; }
		public DbSet<MappingTable> MappingTables { get; set; }
		public DbSet<TwilioSetting> TwilioSettings { get; set; }
		public DbSet<PhoneSettings> PhoneSettings { get; set; }
		public DbSet<Account> Accounts { get; set; }
		public DbSet<Aggregator> Aggregators { get; set; }
		public DbSet<AggregatorAccount> AggregatorAccounts { get; set; }
		public DbSet<Proxy> Proxies { get; set; }
		public DbSet<ProfileProxy> ProfileProxies { get; set; }

		public DbSet<Master> Masters { get; set; }
		public DbSet<Slave> Slaves { get; set; }

		public DbSet<Copier> Copiers { get; set; }
		public DbSet<CopierPosition> CopierPositions { get; set; }
		public DbSet<FixApiCopier> FixApiCopiers { get; set; }
		public DbSet<FixApiCopierPosition> FixApiCopierPositions { get; set; }
		public DbSet<SymbolMapping> SymbolMappings { get; set; }

		public DbSet<Pushing> Pushings { get; set; }
		public DbSet<PushingDetail> PushingDetails { get; set; }
		public DbSet<Spoofing> Spoofings { get; set; }

		public DbSet<StratPosition> Positions { get; set; }
		public DbSet<StratHubArb> StratHubArbs { get; set; }
		public DbSet<StratHubArbPosition> StratHubArbPositions { get; set; }

		public DbSet<MM> MMs { get; set; }
		public DbSet<MarketMaker> MarketMakers { get; set; }
		public DbSet<LatencyArb> LatencyArbs { get; set; }
		public DbSet<LatencyArbPosition> LatencyArbPositions { get; set; }
		public DbSet<NewsArb> NewsArbs { get; set; }
		public DbSet<NewsArbPosition> NewsArbPositions { get; set; }

		public DbSet<Ticker> Tickers { get; set; }
		public DbSet<Export> Exports { get; set; }

		public DbSet<RiskManagementSetting> Settings { get; set; }
		public DbSet<RiskManagement> RiskManagements { get; set; }
		public override int SaveChanges()
		{
			AddRiskManager();
			AddTimestamps();
			return base.SaveChanges();
		}

		public void Init()
		{
			if (bool.TryParse(ConfigurationManager.AppSettings["AllowDatabaseMigration"], out bool migrate) && migrate)
				Database.Migrate();

			try
			{
				Directory.CreateDirectory(".\\Mt4SrvFiles");
				foreach (var name in Directory.GetFiles(".\\Mt4SrvFiles", "*.srv").Select(Path.GetFileNameWithoutExtension))
				{
					if (MetaTraderPlatforms.Any(p => p.Description == name)) continue;
					MetaTraderPlatforms.Add(new MetaTraderPlatform()
					{
						Description = name,
						SrvFilePath = $"Mt4SrvFiles\\{name}.srv"
					});
				}

				SaveChanges();
			}
			catch { }

			try
			{
				Directory.CreateDirectory(".\\FixApiConfigFiles");
				foreach (var name in Directory.GetFiles(".\\FixApiConfigFiles", "*.xml").Select(Path.GetFileNameWithoutExtension))
				{
					if (FixApiAccounts.Any(p => p.Description == name)) continue;
					FixApiAccounts.Add(new FixApiAccount()
					{
						Description = name,
						ConfigPath = $"FixApiConfigFiles\\{name}.xml"
					});
				}
				SaveChanges();
			}
			catch { }

			try
			{
				foreach (var a in Accounts.Where(a => a.RiskManagement == null))
				{
					a.RiskManagement = new RiskManagement { RiskManagementSetting = new RiskManagementSetting() };
				}
				SaveChanges();
			}
			catch { }

			try
			{
				// Create default entities in the TwilioSettings and delete any existing ones
				var accountSid = ConfigurationManager.AppSettings["TwilioService.AccountSid"];
				var authToken = ConfigurationManager.AppSettings["TwilioService.AuthToken"];
				var twilioPhoneNumber = ConfigurationManager.AppSettings["TwilioService.TwilioPhoneNumber"];
				var message = ConfigurationManager.AppSettings["TwilioService.Message"];
				var coolDownTimerInMin = ConfigurationManager.AppSettings["TwilioService.CoolDownTimerInMin"];

				// Check if the default entities already exist
				var existingEntities = TwilioSettings.ToList();

				if (existingEntities.All(entity => entity.Key != accountSid))
				{
					// Add default entity for accountSid
					TwilioSettings.Add(new TwilioSetting { Key = accountSid });
				}

				if (existingEntities.All(entity => entity.Key != authToken))
				{
					// Add default entity for authToken
					TwilioSettings.Add(new TwilioSetting { Key = authToken });
				}

				if (existingEntities.All(entity => entity.Key != twilioPhoneNumber))
				{
					// Add default entity for twilioPhoneNumber
					TwilioSettings.Add(new TwilioSetting { Key = twilioPhoneNumber });
				}

				if (existingEntities.All(entity => entity.Key != message))
				{
					// Add default entity for message
					TwilioSettings.Add(new TwilioSetting { Key = message });
				}

				if (existingEntities.All(entity => entity.Key != coolDownTimerInMin))
				{
					// Add default entity for message
					TwilioSettings.Add(new TwilioSetting { Key = coolDownTimerInMin, Value = "1" });
				}

				// Remove other entities from the database
				foreach (var entity in existingEntities)
				{
					if (entity.Key != accountSid && entity.Key != authToken && entity.Key != twilioPhoneNumber && entity.Key != message && entity.Key != coolDownTimerInMin)
					{
						TwilioSettings.Remove(entity);
					}
				}

				SaveChanges();
			}
			catch { }
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<StratHubArb>().Property(x => x.PipSize).HasPrecision(18, 5);
			modelBuilder.Entity<StratHubArb>().Property(x => x.Size).HasPrecision(18, 3);
			modelBuilder.Entity<StratHubArb>().Property(x => x.SlippageInPip).HasPrecision(18, 2);

			modelBuilder.Entity<FixApiCopier>().Property(x => x.PipSize).HasPrecision(18, 5);
			modelBuilder.Entity<FixApiCopier>().Property(x => x.SlippageInPip).HasPrecision(18, 2);

			modelBuilder.Entity<AggregatorAccount>().HasKey(x => new { x.AggregatorId, x.AccountId });
			//modelBuilder.Entity<AggregatorAccount>()
			//	.HasOne(x => x.Aggregator).WithMany(x => x.Accounts)
			//	.HasForeignKey(x => x.AggregatorId);
			//modelBuilder.Entity<AggregatorAccount>()
			//	.HasOne(x => x.Account).WithMany(x => x.Aggregators)
			//	.HasForeignKey(x => x.AccountId);

			modelBuilder.Entity<StratHubArbPosition>().HasKey(x => new { x.StratHubArbId, x.PositionId });
			//modelBuilder.Entity<StratHubArbPosition>()
			//	.HasOne(x => x.StratHubArb).WithMany(x => x.StratHubArbPositions)
			//	.HasForeignKey(x => x.StratHubArbId);
			//modelBuilder.Entity<StratHubArbPosition>()
			//	.HasOne(x => x.Position).WithMany(x => x.StratHubArbPositions)
			//	.HasForeignKey(x => x.PositionId);

			modelBuilder.Entity<Account>()
			.HasOne(a => a.RiskManagement)
			.WithOne(r => r.Account)
			.HasForeignKey<RiskManagement>(r => r.AccountId)
			.IsRequired()
			.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<RiskManagement>()
			.HasOne(rm => rm.RiskManagementSetting)
			.WithOne(s => s.RiskManagement)
			.HasForeignKey<RiskManagementSetting>(s => s.RiskManagementId)
			.IsRequired()
			.OnDelete(DeleteBehavior.Cascade);

			var timeSpanConverter = new ValueConverter<TimeSpan, long>(v => v.Ticks, v => new TimeSpan(v));
			var nullTimeSpanConverter = new ValueConverter<TimeSpan?, long?>(v => v != null ? v.Value.Ticks : (long?)null,
				v => v != null ? new TimeSpan(v.Value) : (TimeSpan?)null);
			var arrayConverter = new ValueConverter<string[], string>(
				x => JsonConvert.SerializeObject(x),
				x => JsonConvert.DeserializeObject<string[]>(x));

			modelBuilder
				.Entity<FixApiCopierPosition>()
				.Property(e => e.OpenOrderIds)
				.HasConversion(arrayConverter);

			foreach (var entityType in modelBuilder.Model.GetEntityTypes())
				foreach (var property in entityType.GetProperties())
				{
					if (property.ClrType == typeof(TimeSpan))
						property.SetValueConverter(timeSpanConverter);
					else if (property.ClrType == typeof(TimeSpan?))
						property.SetValueConverter(nullTimeSpanConverter);
				}

			//modelBuilder.Entity<RiskManagement>()
			//	.Has
			//.HasRequired(s => s.)
			//.WithOptional()
			//.Map(m => m.MapKey("FirstId"));
		}

		private void AddRiskManager()
		{
			var accountEntities = ChangeTracker.Entries()
				.Where(x => x.State == EntityState.Added && (x.Entity is Account));

			foreach (var acc in accountEntities)
			{
				var rm = new RiskManagement { RiskManagementSetting = new RiskManagementSetting() };
				(acc.Entity as Account).RiskManagement = rm;
			}
		}

		private void AddTimestamps()
		{
			var entities = ChangeTracker.Entries()
				.Where(x => x.Entity is BaseEntity && (x.State == EntityState.Added || x.State == EntityState.Modified));

			foreach (var entity in entities)
			{
				var now = DateTime.UtcNow; // current datetime

				if (entity.State == EntityState.Added)
				{
					((BaseEntity)entity.Entity).CreatedAt = now;
				}
				((BaseEntity)entity.Entity).UpdatedAt = now;
			}
		}
	}
}
