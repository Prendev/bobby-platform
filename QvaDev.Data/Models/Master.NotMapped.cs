using QvaDev.Common.Integration;

namespace QvaDev.Data.Models
{
    public partial class Master
	{
		public event NewPositionEventHandler NewPosition;

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

		private void Account_NewPosition(object sender, NewPositionEventArgs newPositionEventArgs)
			=> NewPosition?.Invoke(this, newPositionEventArgs);

		public override string ToString()
        {
            return (Id == 0 ? "UNSAVED - " : "") + Account;
        }
    }
}
