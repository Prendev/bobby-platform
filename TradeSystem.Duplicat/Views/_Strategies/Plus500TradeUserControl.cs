using System;
using System.Linq;
using System.Windows.Forms;
using TradeSystem.Duplicat.ViewModel;

namespace TradeSystem.Duplicat.Views._Strategies
{
	public partial class Plus500TradeUserControl : UserControl, IMvvmUserControl
	{
		private DuplicatViewModel _viewModel;

		public Plus500TradeUserControl()
		{
			InitializeComponent();
		}

		public void AttachDataSources()
		{
			scdvTrade.SortableDataSource = _viewModel.SortedPlus500TradePositions;

			Timer timer = new Timer();
			timer.Interval = 5000; // Update every second
			timer.Tick += Timer_Tick;
			timer.Start();
		}

		private void Timer_Tick(object sender, EventArgs e)
		{
			//TODO plus500 pos checker
			UpdatePositions();
		}

		private void UpdatePositions()
		{
			if (InvokeRequired)
			{
				Invoke(new Action(UpdatePositions));
				return;
			}

			var Plus500Positions = _viewModel.ConnectedAccounts.Where(a => a.Connector is Plus500Integration.Connector).Select(a => a.Connector as Plus500Integration.Connector).SelectMany(c => c.Plus500Positions).ToList();
			_viewModel.SortedPlus500TradePositions.Clear();
			foreach (var pos in Plus500Positions)
			{
				_viewModel.SortedPlus500TradePositions.Add(pos.Value);
			}
		}

		public void InitView(DuplicatViewModel viewModel)
		{
			_viewModel = viewModel;

			lbTrade.AddBinding<string, bool>("Visible", viewModel, nameof(_viewModel.FilterText), s => string.IsNullOrEmpty(s));
			lbTrade.Click += (s, e) => { tbTrade.Focus(); };

			btnFlush.Click += (s, e) =>
			{
				tbTrade.Text = string.Empty;
				//_viewModel.FlushTrade();
			};

			tbTrade.TextChanged += (sender, e) =>
			{
				//_viewModel.FilterTradePositions((sender as TextBox).Text);
			};
		}
	}
}
