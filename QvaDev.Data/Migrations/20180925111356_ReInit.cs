using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace QvaDev.Data.Migrations
{
    public partial class ReInit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CqgClientApiAccounts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Description = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(nullable: false),
                    Password = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CqgClientApiAccounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CTraderPlatforms",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Description = table.Column<string>(nullable: false),
                    AccountsApi = table.Column<string>(nullable: false),
                    TradingHost = table.Column<string>(nullable: false),
                    ClientId = table.Column<string>(nullable: false),
                    Secret = table.Column<string>(nullable: false),
                    Playground = table.Column<string>(nullable: false),
                    AccessBaseUrl = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CTraderPlatforms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FixApiAccounts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Description = table.Column<string>(nullable: false),
                    ConfigPath = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FixApiAccounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FixTraderAccounts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Description = table.Column<string>(nullable: false),
                    IpAddress = table.Column<string>(nullable: false),
                    CommandSocketPort = table.Column<int>(nullable: false),
                    EventsSocketPort = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FixTraderAccounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IlyaFastFeedAccounts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Description = table.Column<string>(nullable: false),
                    IpAddress = table.Column<string>(nullable: false),
                    Port = table.Column<int>(nullable: false),
                    UserName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IlyaFastFeedAccounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MetaTraderPlatforms",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Description = table.Column<string>(nullable: false),
                    SrvFilePath = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetaTraderPlatforms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Profiles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Description = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PushingDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SmallContractSize = table.Column<int>(nullable: false),
                    BigContractSize = table.Column<int>(nullable: false),
                    BigPercentage = table.Column<int>(nullable: false),
                    FutureOpenDelayInMs = table.Column<int>(nullable: false),
                    MinIntervalInMs = table.Column<int>(nullable: false),
                    MaxIntervalInMs = table.Column<int>(nullable: false),
                    HedgeSignalContractLimit = table.Column<int>(nullable: false),
                    MasterSignalContractLimit = table.Column<int>(nullable: false),
                    EndingMinIntervalInMs = table.Column<int>(nullable: false),
                    EndingMaxIntervalInMs = table.Column<int>(nullable: false),
                    FullContractSize = table.Column<int>(nullable: false),
                    PartialClosePercentage = table.Column<int>(nullable: false),
                    AlphaLots = table.Column<double>(nullable: false),
                    BetaLots = table.Column<double>(nullable: false),
                    HedgeLots = table.Column<double>(nullable: false),
                    MaxRetryCount = table.Column<int>(nullable: false),
                    RetryPeriodInMs = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PushingDetails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CTraderAccounts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Description = table.Column<string>(nullable: false),
                    AccountNumber = table.Column<long>(nullable: false),
                    AccessToken = table.Column<string>(nullable: false),
                    CTraderPlatformId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CTraderAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CTraderAccounts_CTraderPlatforms_CTraderPlatformId",
                        column: x => x.CTraderPlatformId,
                        principalTable: "CTraderPlatforms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MetaTraderAccounts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Description = table.Column<string>(nullable: false),
                    User = table.Column<long>(nullable: false),
                    Password = table.Column<string>(nullable: false),
                    MetaTraderPlatformId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetaTraderAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MetaTraderAccounts_MetaTraderPlatforms_MetaTraderPlatformId",
                        column: x => x.MetaTraderPlatformId,
                        principalTable: "MetaTraderPlatforms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Aggregators",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Description = table.Column<string>(nullable: false),
                    ProfileId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aggregators", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Aggregators_Profiles_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProfileId = table.Column<int>(nullable: false),
                    Run = table.Column<bool>(nullable: false),
                    CTraderAccountId = table.Column<int>(nullable: true),
                    MetaTraderAccountId = table.Column<int>(nullable: true),
                    FixTraderAccountId = table.Column<int>(nullable: true),
                    FixApiAccountId = table.Column<int>(nullable: true),
                    IlyaFastFeedAccountId = table.Column<int>(nullable: true),
                    CqgClientApiAccountId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Accounts_CTraderAccounts_CTraderAccountId",
                        column: x => x.CTraderAccountId,
                        principalTable: "CTraderAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Accounts_CqgClientApiAccounts_CqgClientApiAccountId",
                        column: x => x.CqgClientApiAccountId,
                        principalTable: "CqgClientApiAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Accounts_FixApiAccounts_FixApiAccountId",
                        column: x => x.FixApiAccountId,
                        principalTable: "FixApiAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Accounts_FixTraderAccounts_FixTraderAccountId",
                        column: x => x.FixTraderAccountId,
                        principalTable: "FixTraderAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Accounts_IlyaFastFeedAccounts_IlyaFastFeedAccountId",
                        column: x => x.IlyaFastFeedAccountId,
                        principalTable: "IlyaFastFeedAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Accounts_MetaTraderAccounts_MetaTraderAccountId",
                        column: x => x.MetaTraderAccountId,
                        principalTable: "MetaTraderAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Accounts_Profiles_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StratHubArbs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Description = table.Column<string>(nullable: false),
                    AggregatorId = table.Column<int>(nullable: false),
                    Run = table.Column<bool>(nullable: false),
                    Size = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    MaxSizePerAccount = table.Column<decimal>(nullable: false),
                    PipSize = table.Column<decimal>(type: "decimal(18,5)", nullable: false),
                    SignalDiffInPip = table.Column<decimal>(nullable: false),
                    MinOpenTimeInMinutes = table.Column<int>(nullable: false),
                    EarliestOpenTime = table.Column<long>(nullable: true),
                    LatestOpenTime = table.Column<long>(nullable: true),
                    LatestCloseTime = table.Column<long>(nullable: true),
                    OrderType = table.Column<int>(nullable: false),
                    MaxRetryCount = table.Column<int>(nullable: false),
                    RetryPeriodInMs = table.Column<int>(nullable: false),
                    SlippageInPip = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TimeWindowInMs = table.Column<int>(nullable: false),
                    LastOpenTime = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StratHubArbs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StratHubArbs_Aggregators_AggregatorId",
                        column: x => x.AggregatorId,
                        principalTable: "Aggregators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AggregatorAccounts",
                columns: table => new
                {
                    Aggregator_Id = table.Column<int>(nullable: false),
                    Account_Id = table.Column<int>(nullable: false),
                    Symbol = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AggregatorAccounts", x => new { x.Aggregator_Id, x.Account_Id });
                    table.UniqueConstraint("AK_AggregatorAccounts_Account_Id_Aggregator_Id", x => new { x.Account_Id, x.Aggregator_Id });
                    table.ForeignKey(
                        name: "FK_AggregatorAccounts_Accounts_Account_Id",
                        column: x => x.Account_Id,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AggregatorAccounts_Aggregators_Aggregator_Id",
                        column: x => x.Aggregator_Id,
                        principalTable: "Aggregators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Masters",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProfileId = table.Column<int>(nullable: false),
                    Run = table.Column<bool>(nullable: false),
                    AccountId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Masters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Masters_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Masters_Profiles_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Positions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AccountId = table.Column<int>(nullable: false),
                    Symbol = table.Column<string>(nullable: true),
                    Side = table.Column<int>(nullable: false),
                    Size = table.Column<decimal>(nullable: false),
                    OpenTime = table.Column<DateTime>(nullable: false),
                    AvgPrice = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Positions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Positions_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pushings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Description = table.Column<string>(nullable: false),
                    ProfileId = table.Column<int>(nullable: false),
                    FutureExecutionMode = table.Column<int>(nullable: false),
                    FutureAccountId = table.Column<int>(nullable: false),
                    FutureSymbol = table.Column<string>(nullable: false),
                    AlphaMasterId = table.Column<int>(nullable: false),
                    AlphaSymbol = table.Column<string>(nullable: false),
                    BetaMasterId = table.Column<int>(nullable: false),
                    BetaSymbol = table.Column<string>(nullable: false),
                    HedgeAccountId = table.Column<int>(nullable: false),
                    HedgeSymbol = table.Column<string>(nullable: false),
                    PushingDetailId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pushings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pushings_Accounts_AlphaMasterId",
                        column: x => x.AlphaMasterId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Pushings_Accounts_BetaMasterId",
                        column: x => x.BetaMasterId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Pushings_Accounts_FutureAccountId",
                        column: x => x.FutureAccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Pushings_Accounts_HedgeAccountId",
                        column: x => x.HedgeAccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Pushings_Profiles_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Pushings_PushingDetails_PushingDetailId",
                        column: x => x.PushingDetailId,
                        principalTable: "PushingDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StratDealingArbs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Description = table.Column<string>(nullable: false),
                    ProfileId = table.Column<int>(nullable: false),
                    Run = table.Column<bool>(nullable: false),
                    MaxNumberOfPositions = table.Column<int>(nullable: false),
                    SignalDiffInPip = table.Column<decimal>(nullable: false),
                    SignalStepInPip = table.Column<decimal>(nullable: false),
                    TargetInPip = table.Column<decimal>(nullable: false),
                    MinOpenTimeInMinutes = table.Column<int>(nullable: false),
                    ReOpenIntervalInMinutes = table.Column<int>(nullable: false),
                    EarliestOpenTime = table.Column<long>(nullable: true),
                    LatestOpenTime = table.Column<long>(nullable: true),
                    LatestCloseTime = table.Column<long>(nullable: true),
                    OrderType = table.Column<int>(nullable: false),
                    MaxRetryCount = table.Column<int>(nullable: false),
                    RetryPeriodInMs = table.Column<int>(nullable: false),
                    SlippageInPip = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TimeWindowInMs = table.Column<int>(nullable: false),
                    AlphaAccountId = table.Column<int>(nullable: false),
                    AlphaSymbol = table.Column<string>(nullable: false),
                    AlphaSize = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    BetaAccountId = table.Column<int>(nullable: false),
                    BetaSymbol = table.Column<string>(nullable: false),
                    BetaSize = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    ShiftInPip = table.Column<decimal>(nullable: true),
                    ShiftCalcInterval = table.Column<long>(nullable: false),
                    PipSize = table.Column<decimal>(type: "decimal(18,5)", nullable: false),
                    MagicNumber = table.Column<int>(nullable: false),
                    LastOpenTime = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StratDealingArbs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StratDealingArbs_Accounts_AlphaAccountId",
                        column: x => x.AlphaAccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StratDealingArbs_Accounts_BetaAccountId",
                        column: x => x.BetaAccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StratDealingArbs_Profiles_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tickers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProfileId = table.Column<int>(nullable: false),
                    MainAccountId = table.Column<int>(nullable: false),
                    MainSymbol = table.Column<string>(nullable: true),
                    PairAccountId = table.Column<int>(nullable: true),
                    PairSymbol = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tickers_Accounts_MainAccountId",
                        column: x => x.MainAccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tickers_Accounts_PairAccountId",
                        column: x => x.PairAccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tickers_Profiles_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Slaves",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Run = table.Column<bool>(nullable: false),
                    MasterId = table.Column<int>(nullable: false),
                    AccountId = table.Column<int>(nullable: false),
                    SymbolSuffix = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Slaves", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Slaves_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Slaves_Masters_MasterId",
                        column: x => x.MasterId,
                        principalTable: "Masters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StratHubArbPositions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StratHubArbId = table.Column<int>(nullable: false),
                    PositionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StratHubArbPositions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StratHubArbPositions_Positions_PositionId",
                        column: x => x.PositionId,
                        principalTable: "Positions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StratHubArbPositions_StratHubArbs_StratHubArbId",
                        column: x => x.StratHubArbId,
                        principalTable: "StratHubArbs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StratDealingArbPositions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OpenTime = table.Column<DateTime>(nullable: false),
                    CloseTime = table.Column<DateTime>(nullable: true),
                    AlphaOpenSignal = table.Column<decimal>(type: "decimal(18,5)", nullable: false),
                    AlphaOpenPrice = table.Column<decimal>(type: "decimal(18,5)", nullable: false),
                    AlphaClosePrice = table.Column<decimal>(type: "decimal(18,5)", nullable: true),
                    AlphaCloseSignal = table.Column<decimal>(type: "decimal(18,5)", nullable: true),
                    AlphaSide = table.Column<int>(nullable: false),
                    AlphaSize = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    RemainingAlpha = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    AlphaOrderTicket = table.Column<long>(nullable: true),
                    BetaOpenSignal = table.Column<decimal>(type: "decimal(18,5)", nullable: false),
                    BetaOpenPrice = table.Column<decimal>(type: "decimal(18,5)", nullable: false),
                    BetaClosePrice = table.Column<decimal>(type: "decimal(18,5)", nullable: true),
                    BetaCloseSignal = table.Column<decimal>(type: "decimal(18,5)", nullable: true),
                    BetaSide = table.Column<int>(nullable: false),
                    BetaSize = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    RemainingBeta = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    BetaOrderTicket = table.Column<long>(nullable: true),
                    IsClosed = table.Column<bool>(nullable: false),
                    StratDealingArbId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StratDealingArbPositions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StratDealingArbPositions_StratDealingArbs_StratDealingArbId",
                        column: x => x.StratDealingArbId,
                        principalTable: "StratDealingArbs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Copiers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SlaveId = table.Column<int>(nullable: false),
                    Run = table.Column<bool>(nullable: false),
                    CopyRatio = table.Column<decimal>(nullable: false),
                    OrderType = table.Column<int>(nullable: false),
                    SlippageInPips = table.Column<int>(nullable: false),
                    MaxRetryCount = table.Column<int>(nullable: false),
                    RetryPeriodInMs = table.Column<int>(nullable: false),
                    DelayInMilliseconds = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Copiers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Copiers_Slaves_SlaveId",
                        column: x => x.SlaveId,
                        principalTable: "Slaves",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FixApiCopiers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SlaveId = table.Column<int>(nullable: false),
                    Run = table.Column<bool>(nullable: false),
                    CopyRatio = table.Column<decimal>(nullable: false),
                    OrderType = table.Column<int>(nullable: false),
                    DelayInMilliseconds = table.Column<int>(nullable: false),
                    MaxRetryCount = table.Column<int>(nullable: false),
                    RetryPeriodInMs = table.Column<int>(nullable: false),
                    SlippageInPip = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TimeWindowInMs = table.Column<int>(nullable: false),
                    PipSize = table.Column<decimal>(type: "decimal(18,5)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FixApiCopiers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FixApiCopiers_Slaves_SlaveId",
                        column: x => x.SlaveId,
                        principalTable: "Slaves",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SymbolMappings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SlaveId = table.Column<int>(nullable: false),
                    From = table.Column<string>(nullable: true),
                    To = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SymbolMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SymbolMappings_Slaves_SlaveId",
                        column: x => x.SlaveId,
                        principalTable: "Slaves",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_CTraderAccountId",
                table: "Accounts",
                column: "CTraderAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_CqgClientApiAccountId",
                table: "Accounts",
                column: "CqgClientApiAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_FixApiAccountId",
                table: "Accounts",
                column: "FixApiAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_FixTraderAccountId",
                table: "Accounts",
                column: "FixTraderAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_IlyaFastFeedAccountId",
                table: "Accounts",
                column: "IlyaFastFeedAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_MetaTraderAccountId",
                table: "Accounts",
                column: "MetaTraderAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_ProfileId",
                table: "Accounts",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Aggregators_ProfileId",
                table: "Aggregators",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Copiers_SlaveId",
                table: "Copiers",
                column: "SlaveId");

            migrationBuilder.CreateIndex(
                name: "IX_CTraderAccounts_CTraderPlatformId",
                table: "CTraderAccounts",
                column: "CTraderPlatformId");

            migrationBuilder.CreateIndex(
                name: "IX_FixApiCopiers_SlaveId",
                table: "FixApiCopiers",
                column: "SlaveId");

            migrationBuilder.CreateIndex(
                name: "IX_Masters_AccountId",
                table: "Masters",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Masters_ProfileId",
                table: "Masters",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_MetaTraderAccounts_MetaTraderPlatformId",
                table: "MetaTraderAccounts",
                column: "MetaTraderPlatformId");

            migrationBuilder.CreateIndex(
                name: "IX_Positions_AccountId",
                table: "Positions",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Pushings_AlphaMasterId",
                table: "Pushings",
                column: "AlphaMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_Pushings_BetaMasterId",
                table: "Pushings",
                column: "BetaMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_Pushings_FutureAccountId",
                table: "Pushings",
                column: "FutureAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Pushings_HedgeAccountId",
                table: "Pushings",
                column: "HedgeAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Pushings_ProfileId",
                table: "Pushings",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Pushings_PushingDetailId",
                table: "Pushings",
                column: "PushingDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_Slaves_AccountId",
                table: "Slaves",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Slaves_MasterId",
                table: "Slaves",
                column: "MasterId");

            migrationBuilder.CreateIndex(
                name: "IX_StratDealingArbPositions_StratDealingArbId",
                table: "StratDealingArbPositions",
                column: "StratDealingArbId");

            migrationBuilder.CreateIndex(
                name: "IX_StratDealingArbs_AlphaAccountId",
                table: "StratDealingArbs",
                column: "AlphaAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_StratDealingArbs_BetaAccountId",
                table: "StratDealingArbs",
                column: "BetaAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_StratDealingArbs_ProfileId",
                table: "StratDealingArbs",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_StratHubArbPositions_PositionId",
                table: "StratHubArbPositions",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_StratHubArbPositions_StratHubArbId",
                table: "StratHubArbPositions",
                column: "StratHubArbId");

            migrationBuilder.CreateIndex(
                name: "IX_StratHubArbs_AggregatorId",
                table: "StratHubArbs",
                column: "AggregatorId");

            migrationBuilder.CreateIndex(
                name: "IX_SymbolMappings_SlaveId",
                table: "SymbolMappings",
                column: "SlaveId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickers_MainAccountId",
                table: "Tickers",
                column: "MainAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickers_PairAccountId",
                table: "Tickers",
                column: "PairAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickers_ProfileId",
                table: "Tickers",
                column: "ProfileId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AggregatorAccounts");

            migrationBuilder.DropTable(
                name: "Copiers");

            migrationBuilder.DropTable(
                name: "FixApiCopiers");

            migrationBuilder.DropTable(
                name: "Pushings");

            migrationBuilder.DropTable(
                name: "StratDealingArbPositions");

            migrationBuilder.DropTable(
                name: "StratHubArbPositions");

            migrationBuilder.DropTable(
                name: "SymbolMappings");

            migrationBuilder.DropTable(
                name: "Tickers");

            migrationBuilder.DropTable(
                name: "PushingDetails");

            migrationBuilder.DropTable(
                name: "StratDealingArbs");

            migrationBuilder.DropTable(
                name: "Positions");

            migrationBuilder.DropTable(
                name: "StratHubArbs");

            migrationBuilder.DropTable(
                name: "Slaves");

            migrationBuilder.DropTable(
                name: "Aggregators");

            migrationBuilder.DropTable(
                name: "Masters");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "CTraderAccounts");

            migrationBuilder.DropTable(
                name: "CqgClientApiAccounts");

            migrationBuilder.DropTable(
                name: "FixApiAccounts");

            migrationBuilder.DropTable(
                name: "FixTraderAccounts");

            migrationBuilder.DropTable(
                name: "IlyaFastFeedAccounts");

            migrationBuilder.DropTable(
                name: "MetaTraderAccounts");

            migrationBuilder.DropTable(
                name: "Profiles");

            migrationBuilder.DropTable(
                name: "CTraderPlatforms");

            migrationBuilder.DropTable(
                name: "MetaTraderPlatforms");
        }
    }
}
