using System;
using TradeSystem.Common.Integration;

namespace TradeSystem.Data.Models
{
    public partial class Master
	{
		public event EventHandler<NewPosition> NewPosition;

		public Master()
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
            return (Id == 0 ? "UNSAVED - " : "") + Account;
        }
    }
}
