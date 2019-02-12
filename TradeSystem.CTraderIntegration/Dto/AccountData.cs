namespace TradeSystem.CTraderIntegration.Dto
{
    public class AccountData
    {
        public long accountId { get; set; }
        public long accountNumber { get; set; }
        public bool live { get; set; }
        public string brokerName { get; set; }
        public string brokerTitle { get; set; }
        public string depositCurrency { get; set; }
        public string traderRegistrationTimestamp { get; set; }
        public string traderAccountType { get; set; }
        public int leverage { get; set; }
        public int leverageInCents { get; set; }
        public decimal balance { get; set; }
        public bool deleted { get; set; }
        public string accountStatus { get; set; }
        public bool swapFree { get; set; }
    }
}
