namespace TradeSystem.FileContextCore
{
	public static class FileContextStrings
	{
		public static string LogSavedChanges = "Saved {count} entities to in-memory store.";
		public static string LogTransactionsNotSupported = "Transactions are not supported by the in-memory store. See http://go.microsoft.com/fwlink/?LinkId=800142";
		public static string UpdateConcurrencyException = "Attempted to update or delete an entity that does not exist in the store.";
	}
}
