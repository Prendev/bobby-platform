﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using QvaDev.Data;

namespace QvaDev.Data.Migrations
{
    [DbContext(typeof(DuplicatContext))]
    partial class DuplicatContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.3-rtm-32065")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("QvaDev.Data.Models.Account", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("CTraderAccountId");

                    b.Property<int?>("CqgClientApiAccountId");

                    b.Property<int?>("FixApiAccountId");

                    b.Property<int?>("FixTraderAccountId");

                    b.Property<int?>("IlyaFastFeedAccountId");

                    b.Property<int?>("MetaTraderAccountId");

                    b.Property<int>("ProfileId");

                    b.Property<bool>("Run");

                    b.HasKey("Id");

                    b.HasIndex("CTraderAccountId");

                    b.HasIndex("CqgClientApiAccountId");

                    b.HasIndex("FixApiAccountId");

                    b.HasIndex("FixTraderAccountId");

                    b.HasIndex("IlyaFastFeedAccountId");

                    b.HasIndex("MetaTraderAccountId");

                    b.HasIndex("ProfileId");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("QvaDev.Data.Models.Aggregator", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description")
                        .IsRequired();

                    b.Property<int>("ProfileId");

                    b.Property<bool>("Run");

                    b.HasKey("Id");

                    b.HasIndex("ProfileId");

                    b.ToTable("Aggregators");
                });

            modelBuilder.Entity("QvaDev.Data.Models.AggregatorAccount", b =>
                {
                    b.Property<int>("AggregatorId");

                    b.Property<int>("AccountId");

                    b.Property<string>("Symbol")
                        .IsRequired();

                    b.HasKey("AggregatorId", "AccountId");

                    b.HasAlternateKey("AccountId", "AggregatorId");

                    b.ToTable("AggregatorAccounts");
                });

            modelBuilder.Entity("QvaDev.Data.Models.Copier", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal>("CopyRatio");

                    b.Property<int>("DelayInMilliseconds");

                    b.Property<int>("MaxRetryCount");

                    b.Property<int>("OrderType");

                    b.Property<int>("RetryPeriodInMs");

                    b.Property<bool>("Run");

                    b.Property<int>("SlaveId");

                    b.Property<int>("SlippageInPips");

                    b.HasKey("Id");

                    b.HasIndex("SlaveId");

                    b.ToTable("Copiers");
                });

            modelBuilder.Entity("QvaDev.Data.Models.CqgClientApiAccount", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description")
                        .IsRequired();

                    b.Property<string>("Password")
                        .IsRequired();

                    b.Property<string>("UserName")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("CqgClientApiAccounts");
                });

            modelBuilder.Entity("QvaDev.Data.Models.CTraderAccount", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AccessToken")
                        .IsRequired();

                    b.Property<long>("AccountNumber");

                    b.Property<int>("CTraderPlatformId");

                    b.Property<string>("Description")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("CTraderPlatformId");

                    b.ToTable("CTraderAccounts");
                });

            modelBuilder.Entity("QvaDev.Data.Models.CTraderPlatform", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AccessBaseUrl")
                        .IsRequired();

                    b.Property<string>("AccountsApi")
                        .IsRequired();

                    b.Property<string>("ClientId")
                        .IsRequired();

                    b.Property<string>("Description")
                        .IsRequired();

                    b.Property<string>("Playground")
                        .IsRequired();

                    b.Property<string>("Secret")
                        .IsRequired();

                    b.Property<string>("TradingHost")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("CTraderPlatforms");
                });

            modelBuilder.Entity("QvaDev.Data.Models.FixApiAccount", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ConfigPath");

                    b.Property<string>("Description")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("FixApiAccounts");
                });

            modelBuilder.Entity("QvaDev.Data.Models.FixApiCopier", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal>("CopyRatio");

                    b.Property<int>("DelayInMilliseconds");

                    b.Property<int>("MaxRetryCount");

                    b.Property<int>("OrderType");

                    b.Property<decimal>("PipSize")
                        .HasColumnType("decimal(18,5)");

                    b.Property<int>("RetryPeriodInMs");

                    b.Property<bool>("Run");

                    b.Property<int>("SlaveId");

                    b.Property<decimal>("SlippageInPip")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("TimeWindowInMs");

                    b.HasKey("Id");

                    b.HasIndex("SlaveId");

                    b.ToTable("FixApiCopiers");
                });

            modelBuilder.Entity("QvaDev.Data.Models.FixTraderAccount", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CommandSocketPort");

                    b.Property<string>("Description")
                        .IsRequired();

                    b.Property<int>("EventsSocketPort");

                    b.Property<string>("IpAddress")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("FixTraderAccounts");
                });

            modelBuilder.Entity("QvaDev.Data.Models.IlyaFastFeedAccount", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description")
                        .IsRequired();

                    b.Property<string>("IpAddress")
                        .IsRequired();

                    b.Property<int>("Port");

                    b.Property<string>("UserName");

                    b.HasKey("Id");

                    b.ToTable("IlyaFastFeedAccounts");
                });

            modelBuilder.Entity("QvaDev.Data.Models.Master", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AccountId");

                    b.Property<int>("ProfileId");

                    b.Property<bool>("Run");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.HasIndex("ProfileId");

                    b.ToTable("Masters");
                });

            modelBuilder.Entity("QvaDev.Data.Models.MetaTraderAccount", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description")
                        .IsRequired();

                    b.Property<int>("MetaTraderPlatformId");

                    b.Property<string>("Password")
                        .IsRequired();

                    b.Property<long>("User");

                    b.HasKey("Id");

                    b.HasIndex("MetaTraderPlatformId");

                    b.ToTable("MetaTraderAccounts");
                });

            modelBuilder.Entity("QvaDev.Data.Models.MetaTraderPlatform", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description")
                        .IsRequired();

                    b.Property<string>("SrvFilePath");

                    b.HasKey("Id");

                    b.ToTable("MetaTraderPlatforms");
                });

            modelBuilder.Entity("QvaDev.Data.Models.Profile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Profiles");
                });

            modelBuilder.Entity("QvaDev.Data.Models.Pushing", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AlphaMasterId");

                    b.Property<string>("AlphaSymbol")
                        .IsRequired();

                    b.Property<int>("BetaMasterId");

                    b.Property<string>("BetaSymbol")
                        .IsRequired();

                    b.Property<string>("Description")
                        .IsRequired();

                    b.Property<int>("FutureAccountId");

                    b.Property<int>("FutureExecutionMode");

                    b.Property<string>("FutureSymbol")
                        .IsRequired();

                    b.Property<int>("HedgeAccountId");

                    b.Property<string>("HedgeSymbol")
                        .IsRequired();

                    b.Property<int>("ProfileId");

                    b.Property<int>("PushingDetailId");

                    b.HasKey("Id");

                    b.HasIndex("AlphaMasterId");

                    b.HasIndex("BetaMasterId");

                    b.HasIndex("FutureAccountId");

                    b.HasIndex("HedgeAccountId");

                    b.HasIndex("ProfileId");

                    b.HasIndex("PushingDetailId");

                    b.ToTable("Pushings");
                });

            modelBuilder.Entity("QvaDev.Data.Models.PushingDetail", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<double>("AlphaLots");

                    b.Property<double>("BetaLots");

                    b.Property<int>("BigContractSize");

                    b.Property<int>("BigPercentage");

                    b.Property<int>("EndingMaxIntervalInMs");

                    b.Property<int>("EndingMinIntervalInMs");

                    b.Property<int>("FullContractSize");

                    b.Property<int>("FutureOpenDelayInMs");

                    b.Property<double>("HedgeLots");

                    b.Property<int>("HedgeSignalContractLimit");

                    b.Property<int>("MasterSignalContractLimit");

                    b.Property<int>("MaxIntervalInMs");

                    b.Property<int>("MaxRetryCount");

                    b.Property<int>("MinIntervalInMs");

                    b.Property<int>("PartialClosePercentage");

                    b.Property<int>("RetryPeriodInMs");

                    b.Property<int>("SmallContractSize");

                    b.HasKey("Id");

                    b.ToTable("PushingDetails");
                });

            modelBuilder.Entity("QvaDev.Data.Models.Slave", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AccountId");

                    b.Property<int>("MasterId");

                    b.Property<bool>("Run");

                    b.Property<string>("SymbolSuffix");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.HasIndex("MasterId");

                    b.ToTable("Slaves");
                });

            modelBuilder.Entity("QvaDev.Data.Models.StratDealingArb", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AlphaAccountId");

                    b.Property<decimal>("AlphaSize")
                        .HasColumnType("decimal(18,3)");

                    b.Property<string>("AlphaSymbol")
                        .IsRequired();

                    b.Property<int>("BetaAccountId");

                    b.Property<decimal>("BetaSize")
                        .HasColumnType("decimal(18,3)");

                    b.Property<string>("BetaSymbol")
                        .IsRequired();

                    b.Property<string>("Description")
                        .IsRequired();

                    b.Property<long?>("EarliestOpenTime");

                    b.Property<DateTime?>("LastOpenTime");

                    b.Property<long?>("LatestCloseTime");

                    b.Property<long?>("LatestOpenTime");

                    b.Property<int>("MagicNumber");

                    b.Property<int>("MaxNumberOfPositions");

                    b.Property<int>("MaxRetryCount");

                    b.Property<int>("MinOpenTimeInMinutes");

                    b.Property<int>("OrderType");

                    b.Property<decimal>("PipSize")
                        .HasColumnType("decimal(18,5)");

                    b.Property<int>("ProfileId");

                    b.Property<int>("ReOpenIntervalInMinutes");

                    b.Property<int>("RetryPeriodInMs");

                    b.Property<bool>("Run");

                    b.Property<long>("ShiftCalcInterval");

                    b.Property<decimal?>("ShiftInPip");

                    b.Property<decimal>("SignalDiffInPip");

                    b.Property<decimal>("SignalStepInPip");

                    b.Property<decimal>("SlippageInPip")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("TargetInPip");

                    b.Property<int>("TimeWindowInMs");

                    b.HasKey("Id");

                    b.HasIndex("AlphaAccountId");

                    b.HasIndex("BetaAccountId");

                    b.HasIndex("ProfileId");

                    b.ToTable("StratDealingArbs");
                });

            modelBuilder.Entity("QvaDev.Data.Models.StratDealingArbPosition", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal?>("AlphaClosePrice")
                        .HasColumnType("decimal(18,5)");

                    b.Property<decimal?>("AlphaCloseSignal")
                        .HasColumnType("decimal(18,5)");

                    b.Property<decimal>("AlphaOpenPrice")
                        .HasColumnType("decimal(18,5)");

                    b.Property<decimal>("AlphaOpenSignal")
                        .HasColumnType("decimal(18,5)");

                    b.Property<long?>("AlphaOrderTicket");

                    b.Property<int>("AlphaSide");

                    b.Property<decimal>("AlphaSize")
                        .HasColumnType("decimal(18,3)");

                    b.Property<decimal?>("BetaClosePrice")
                        .HasColumnType("decimal(18,5)");

                    b.Property<decimal?>("BetaCloseSignal")
                        .HasColumnType("decimal(18,5)");

                    b.Property<decimal>("BetaOpenPrice")
                        .HasColumnType("decimal(18,5)");

                    b.Property<decimal>("BetaOpenSignal")
                        .HasColumnType("decimal(18,5)");

                    b.Property<long?>("BetaOrderTicket");

                    b.Property<int>("BetaSide");

                    b.Property<decimal>("BetaSize")
                        .HasColumnType("decimal(18,3)");

                    b.Property<DateTime?>("CloseTime");

                    b.Property<bool>("IsClosed");

                    b.Property<DateTime>("OpenTime");

                    b.Property<decimal?>("RemainingAlpha")
                        .HasColumnType("decimal(18,3)");

                    b.Property<decimal?>("RemainingBeta")
                        .HasColumnType("decimal(18,3)");

                    b.Property<int>("StratDealingArbId");

                    b.HasKey("Id");

                    b.HasIndex("StratDealingArbId");

                    b.ToTable("StratDealingArbPositions");
                });

            modelBuilder.Entity("QvaDev.Data.Models.StratHubArb", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AggregatorId");

                    b.Property<string>("Description")
                        .IsRequired();

                    b.Property<long?>("EarliestOpenTime");

                    b.Property<DateTime?>("LastOpenTime");

                    b.Property<long?>("LatestCloseTime");

                    b.Property<long?>("LatestOpenTime");

                    b.Property<int>("MaxRetryCount");

                    b.Property<decimal>("MaxSizePerAccount");

                    b.Property<int>("MinOpenTimeInMinutes");

                    b.Property<int>("OrderType");

                    b.Property<decimal>("PipSize")
                        .HasColumnType("decimal(18,5)");

                    b.Property<int>("RetryPeriodInMs");

                    b.Property<bool>("Run");

                    b.Property<decimal>("SignalDiffInPip");

                    b.Property<decimal>("Size")
                        .HasColumnType("decimal(18,3)");

                    b.Property<decimal>("SlippageInPip")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("TimeWindowInMs");

                    b.HasKey("Id");

                    b.HasIndex("AggregatorId");

                    b.ToTable("StratHubArbs");
                });

            modelBuilder.Entity("QvaDev.Data.Models.StratHubArbPosition", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("PositionId");

                    b.Property<int>("StratHubArbId");

                    b.HasKey("Id");

                    b.HasIndex("PositionId");

                    b.HasIndex("StratHubArbId");

                    b.ToTable("StratHubArbPositions");
                });

            modelBuilder.Entity("QvaDev.Data.Models.StratPosition", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AccountId");

                    b.Property<decimal>("AvgPrice");

                    b.Property<DateTime>("OpenTime");

                    b.Property<int>("Side");

                    b.Property<decimal>("Size");

                    b.Property<string>("Symbol");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.ToTable("Positions");
                });

            modelBuilder.Entity("QvaDev.Data.Models.SymbolMapping", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("From");

                    b.Property<int>("SlaveId");

                    b.Property<string>("To");

                    b.HasKey("Id");

                    b.HasIndex("SlaveId");

                    b.ToTable("SymbolMappings");
                });

            modelBuilder.Entity("QvaDev.Data.Models.Ticker", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("MainAccountId");

                    b.Property<string>("MainSymbol");

                    b.Property<int>("MarketDepth");

                    b.Property<int?>("PairAccountId");

                    b.Property<string>("PairSymbol");

                    b.Property<int>("ProfileId");

                    b.HasKey("Id");

                    b.HasIndex("MainAccountId");

                    b.HasIndex("PairAccountId");

                    b.HasIndex("ProfileId");

                    b.ToTable("Tickers");
                });

            modelBuilder.Entity("QvaDev.Data.Models.Account", b =>
                {
                    b.HasOne("QvaDev.Data.Models.CTraderAccount", "CTraderAccount")
                        .WithMany("Accounts")
                        .HasForeignKey("CTraderAccountId");

                    b.HasOne("QvaDev.Data.Models.CqgClientApiAccount", "CqgClientApiAccount")
                        .WithMany("Accounts")
                        .HasForeignKey("CqgClientApiAccountId");

                    b.HasOne("QvaDev.Data.Models.FixApiAccount", "FixApiAccount")
                        .WithMany("Accounts")
                        .HasForeignKey("FixApiAccountId");

                    b.HasOne("QvaDev.Data.Models.FixTraderAccount", "FixTraderAccount")
                        .WithMany("Accounts")
                        .HasForeignKey("FixTraderAccountId");

                    b.HasOne("QvaDev.Data.Models.IlyaFastFeedAccount", "IlyaFastFeedAccount")
                        .WithMany("Accounts")
                        .HasForeignKey("IlyaFastFeedAccountId");

                    b.HasOne("QvaDev.Data.Models.MetaTraderAccount", "MetaTraderAccount")
                        .WithMany("Accounts")
                        .HasForeignKey("MetaTraderAccountId");

                    b.HasOne("QvaDev.Data.Models.Profile", "Profile")
                        .WithMany("Accounts")
                        .HasForeignKey("ProfileId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("QvaDev.Data.Models.Aggregator", b =>
                {
                    b.HasOne("QvaDev.Data.Models.Profile", "Profile")
                        .WithMany()
                        .HasForeignKey("ProfileId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("QvaDev.Data.Models.AggregatorAccount", b =>
                {
                    b.HasOne("QvaDev.Data.Models.Account", "Account")
                        .WithMany("Aggregators")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("QvaDev.Data.Models.Aggregator", "Aggregator")
                        .WithMany("Accounts")
                        .HasForeignKey("AggregatorId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("QvaDev.Data.Models.Copier", b =>
                {
                    b.HasOne("QvaDev.Data.Models.Slave", "Slave")
                        .WithMany("Copiers")
                        .HasForeignKey("SlaveId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("QvaDev.Data.Models.CTraderAccount", b =>
                {
                    b.HasOne("QvaDev.Data.Models.CTraderPlatform", "CTraderPlatform")
                        .WithMany()
                        .HasForeignKey("CTraderPlatformId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("QvaDev.Data.Models.FixApiCopier", b =>
                {
                    b.HasOne("QvaDev.Data.Models.Slave", "Slave")
                        .WithMany("FixApiCopiers")
                        .HasForeignKey("SlaveId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("QvaDev.Data.Models.Master", b =>
                {
                    b.HasOne("QvaDev.Data.Models.Account", "Account")
                        .WithMany()
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("QvaDev.Data.Models.Profile", "Profile")
                        .WithMany("Masters")
                        .HasForeignKey("ProfileId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("QvaDev.Data.Models.MetaTraderAccount", b =>
                {
                    b.HasOne("QvaDev.Data.Models.MetaTraderPlatform", "MetaTraderPlatform")
                        .WithMany()
                        .HasForeignKey("MetaTraderPlatformId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("QvaDev.Data.Models.Pushing", b =>
                {
                    b.HasOne("QvaDev.Data.Models.Account", "AlphaMaster")
                        .WithMany()
                        .HasForeignKey("AlphaMasterId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("QvaDev.Data.Models.Account", "BetaMaster")
                        .WithMany()
                        .HasForeignKey("BetaMasterId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("QvaDev.Data.Models.Account", "FutureAccount")
                        .WithMany()
                        .HasForeignKey("FutureAccountId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("QvaDev.Data.Models.Account", "HedgeAccount")
                        .WithMany()
                        .HasForeignKey("HedgeAccountId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("QvaDev.Data.Models.Profile", "Profile")
                        .WithMany("Pushings")
                        .HasForeignKey("ProfileId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("QvaDev.Data.Models.PushingDetail", "PushingDetail")
                        .WithMany()
                        .HasForeignKey("PushingDetailId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("QvaDev.Data.Models.Slave", b =>
                {
                    b.HasOne("QvaDev.Data.Models.Account", "Account")
                        .WithMany()
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("QvaDev.Data.Models.Master", "Master")
                        .WithMany("Slaves")
                        .HasForeignKey("MasterId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("QvaDev.Data.Models.StratDealingArb", b =>
                {
                    b.HasOne("QvaDev.Data.Models.Account", "AlphaAccount")
                        .WithMany()
                        .HasForeignKey("AlphaAccountId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("QvaDev.Data.Models.Account", "BetaAccount")
                        .WithMany()
                        .HasForeignKey("BetaAccountId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("QvaDev.Data.Models.Profile", "Profile")
                        .WithMany()
                        .HasForeignKey("ProfileId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("QvaDev.Data.Models.StratDealingArbPosition", b =>
                {
                    b.HasOne("QvaDev.Data.Models.StratDealingArb", "StratDealingArb")
                        .WithMany("Positions")
                        .HasForeignKey("StratDealingArbId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("QvaDev.Data.Models.StratHubArb", b =>
                {
                    b.HasOne("QvaDev.Data.Models.Aggregator", "Aggregator")
                        .WithMany()
                        .HasForeignKey("AggregatorId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("QvaDev.Data.Models.StratHubArbPosition", b =>
                {
                    b.HasOne("QvaDev.Data.Models.StratPosition", "Position")
                        .WithMany()
                        .HasForeignKey("PositionId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("QvaDev.Data.Models.StratHubArb", "StratHubArb")
                        .WithMany("StratHubArbPositions")
                        .HasForeignKey("StratHubArbId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("QvaDev.Data.Models.StratPosition", b =>
                {
                    b.HasOne("QvaDev.Data.Models.Account", "Account")
                        .WithMany()
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("QvaDev.Data.Models.SymbolMapping", b =>
                {
                    b.HasOne("QvaDev.Data.Models.Slave", "Slave")
                        .WithMany("SymbolMappings")
                        .HasForeignKey("SlaveId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("QvaDev.Data.Models.Ticker", b =>
                {
                    b.HasOne("QvaDev.Data.Models.Account", "MainAccount")
                        .WithMany()
                        .HasForeignKey("MainAccountId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("QvaDev.Data.Models.Account", "PairAccount")
                        .WithMany()
                        .HasForeignKey("PairAccountId");

                    b.HasOne("QvaDev.Data.Models.Profile", "Profile")
                        .WithMany()
                        .HasForeignKey("ProfileId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
