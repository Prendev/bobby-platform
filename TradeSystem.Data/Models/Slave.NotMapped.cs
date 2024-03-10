using System;
using TradeSystem.Common.Integration;

namespace TradeSystem.Data.Models
{
    public partial class Slave
	{
		public event EventHandler<NewPosition> NewPosition;

		public Slave()
		{
			SetAction<Account>(nameof(Account),
				x =>
				{
					if (x == null) return;
					x.NewPosition -= Account_NewPosition;
				},
				x =>
				{
					if (x == null) return;
					x.NewPosition += Account_NewPosition;
				});
		}

		private void Account_NewPosition(object sender, NewPosition newPosition)
			=> NewPosition?.Invoke(this, newPosition);

		public override string ToString()
		{
			var description = string.IsNullOrEmpty(Description) ? $"{Master} - {Account}" : Description;
			return $"{(Id == 0 ? "UNSAVED - " : "")}{description}";

		}
	}
}
