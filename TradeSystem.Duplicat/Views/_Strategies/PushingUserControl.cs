using System;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using TradeSystem.Common.Integration;
using TradeSystem.Data.Models;
using TradeSystem.Duplicat.ViewModel;

namespace TradeSystem.Duplicat.Views
{
    public partial class PushingUserControl : UserControl, IMvvmUserControl
	{
        private DuplicatViewModel _viewModel;

        public PushingUserControl()
        {
            InitializeComponent();
        }

        public void InitView(DuplicatViewModel viewModel)
        {
            _viewModel = viewModel;

            gbControl.AddBinding("Enabled", _viewModel, nameof(_viewModel.IsLoading), true);
            gbFlow.AddBinding("Enabled", _viewModel, nameof(_viewModel.IsPushingEnabled));
            dgvPushings.AddBinding("AllowUserToAddRows", _viewModel, nameof(_viewModel.IsConfigReadonly), true);
            dgvPushings.AddBinding("AllowUserToDeleteRows", _viewModel, nameof(_viewModel.IsConfigReadonly), true);
			btnStartCopiers.AddBinding("Enabled", _viewModel, nameof(_viewModel.AreCopiersStarted), true);
			btnStopCopiers.AddBinding("Enabled", _viewModel, nameof(_viewModel.AreCopiersStarted));

			gbPushings.AddBinding<Pushing, string>("Text", _viewModel, nameof(_viewModel.SelectedPushing),
				s => $"Pushings (use double-click) - {s?.ToString() ?? "Save before load!!!"}");

			btnBuyBeta.AddBinding<DuplicatViewModel.PushingStates>("Enabled", _viewModel,
                nameof(_viewModel.PushingState), p => p == DuplicatViewModel.PushingStates.NotRunning);
            btnSellBeta.AddBinding<DuplicatViewModel.PushingStates>("Enabled", _viewModel,
                nameof(_viewModel.PushingState), p => p == DuplicatViewModel.PushingStates.NotRunning);
	        btnRushOpenPull.AddBinding<DuplicatViewModel.PushingStates>("Enabled", _viewModel,
		        nameof(_viewModel.PushingState), p => p == DuplicatViewModel.PushingStates.AfterOpeningBeta);
			btnRushOpen.AddBinding<DuplicatViewModel.PushingStates>("Enabled", _viewModel,
                nameof(_viewModel.PushingState), p => p == DuplicatViewModel.PushingStates.AfterOpeningPull);
			btnRushOpenFinish.AddBinding<DuplicatViewModel.PushingStates>("Enabled", _viewModel,
				nameof(_viewModel.PushingState), p => p == DuplicatViewModel.PushingStates.AfterOpeningAlpha);

			btnCloseLongSellFutures.AddBinding<DuplicatViewModel.PushingStates>("Enabled", _viewModel,
                nameof(_viewModel.PushingState), p => p == DuplicatViewModel.PushingStates.BeforeClosing);
            btnCloseShortBuyFutures.AddBinding<DuplicatViewModel.PushingStates>("Enabled", _viewModel,
                nameof(_viewModel.PushingState), p => p == DuplicatViewModel.PushingStates.BeforeClosing);
	        btnRushClosePull.AddBinding<DuplicatViewModel.PushingStates>("Enabled", _viewModel,
		        nameof(_viewModel.PushingState), p => p == DuplicatViewModel.PushingStates.AfterClosingFirst);
			btnRushHedge.AddBinding<DuplicatViewModel.PushingStates>("Enabled", _viewModel,
                nameof(_viewModel.PushingState), p => p == DuplicatViewModel.PushingStates.AfterClosingPull);
			btnRushClose.AddBinding<DuplicatViewModel.PushingStates>("Enabled", _viewModel,
				nameof(_viewModel.PushingState), p => p == DuplicatViewModel.PushingStates.AfterOpeningHedge);
			btnRushCloseFinish.AddBinding<DuplicatViewModel.PushingStates>("Enabled", _viewModel,
				nameof(_viewModel.PushingState), p => p == DuplicatViewModel.PushingStates.AfterClosingSecond);

			dgvPushings.DefaultValuesNeeded += (s, e) =>
            {
                e.Row.Cells["ProfileId"].Value = _viewModel.SelectedProfile.Id;
                e.Row.Cells["PushingDetail"].Value = new PushingDetail();
            };

			btnStartCopiers.Click += (s, e) => { _viewModel.StartCopiersCommand(); };
			btnStopCopiers.Click += (s, e) => { _viewModel.StopCopiersCommand(); };
			btnSubscribeFeed.Click += (s, e) => { _viewModel.PushingFeedSubscribe(dgvPushings.GetSelectedItem<Pushing>()); };

			btnBuyFutures.Click += (s, e) =>
			{
				var pushing = dgvPushings.GetSelectedItem<Pushing>();
				_viewModel.SendOrderCommand(pushing.FutureAccount, Sides.Buy, pushing.FutureSymbol, nudFuturesContractSize.Value);
	        };
	        btnSellFutures.Click += (s, e) =>
			{
				var pushing = dgvPushings.GetSelectedItem<Pushing>();
				_viewModel.SendOrderCommand(pushing.FutureAccount, Sides.Sell, pushing.FutureSymbol, nudFuturesContractSize.Value);
	        };

			btnBuyBeta.Click += (s, e) => { _viewModel.PushingOpenCommand(dgvPushings.GetSelectedItem<Pushing>(), Sides.Buy); };
            btnSellBeta.Click += (s, e) => { _viewModel.PushingOpenCommand(dgvPushings.GetSelectedItem<Pushing>(), Sides.Sell); };
	        btnRushOpenPull.Click += (s, e) => { _viewModel.PushingPanicCommand(dgvPushings.GetSelectedItem<Pushing>()); };
			btnRushOpen.Click += (s, e) => { _viewModel.PushingPanicCommand(dgvPushings.GetSelectedItem<Pushing>()); };
			btnRushOpenFinish.Click += (s, e) => { _viewModel.PushingPanicCommand(dgvPushings.GetSelectedItem<Pushing>()); };

			btnCloseLongSellFutures.Click += (s, e) => { _viewModel.PushingCloseCommand(dgvPushings.GetSelectedItem<Pushing>(), Sides.Buy); };
            btnCloseShortBuyFutures.Click += (s, e) => { _viewModel.PushingCloseCommand(dgvPushings.GetSelectedItem<Pushing>(), Sides.Sell); };
            btnRushClosePull.Click += (s, e) => { _viewModel.PushingPanicCommand(dgvPushings.GetSelectedItem<Pushing>()); };
            btnRushHedge.Click += (s, e) => { _viewModel.PushingPanicCommand(dgvPushings.GetSelectedItem<Pushing>()); };
			btnRushClose.Click += (s, e) => { _viewModel.PushingPanicCommand(dgvPushings.GetSelectedItem<Pushing>()); };
			btnRushCloseFinish.Click += (s, e) => { _viewModel.PushingPanicCommand(dgvPushings.GetSelectedItem<Pushing>()); };
			btnReset.Click += (s, e) => { _viewModel.PushingResetCommand(); };

			dgvPushings.RowDoubleClick += Load_Click;
		}

		private void Load_Click(object sender, EventArgs e)
		{
			if (_viewModel.IsConfigReadonly) return;
			var pushing = dgvPushings.GetSelectedItem<Pushing>();
			_viewModel.ShowPushingCommand(pushing);
			dgvPushingDetail.DataSource = new ObservableCollection<PushingDetail> { pushing.PushingDetail};

			cbHedge.DataBindings.Clear();
			cbHedge.AddBinding("Checked", pushing, nameof(pushing.IsHedgeClose));

			//btnSubscribeFeed.DataBindings.Clear();
			//btnSubscribeFeed.AddBinding<Tick, string>("Text", pushing,
			//	nameof(pushing.LastFeedTick), p => p == null ? "Subscribe feed" : $"Ask: {p.Ask}\nBid: {p.Bid}");
		}

		public void AttachDataSources()
        {
            dgvPushings.AddComboBoxColumn(_viewModel.Accounts, "FeedAccount");
            dgvPushings.AddComboBoxColumn(_viewModel.Accounts, "SpoofAccount");
            dgvPushings.AddComboBoxColumn(_viewModel.Accounts, "FutureAccount");
            dgvPushings.AddComboBoxColumn(_viewModel.Accounts, "AlphaMaster");
            dgvPushings.AddComboBoxColumn(_viewModel.Accounts, "BetaMaster");
            dgvPushings.AddComboBoxColumn(_viewModel.Accounts, "HedgeAccount");
            dgvPushings.AddComboBoxColumn(_viewModel.Accounts, "ReopenAccount");
            dgvPushings.DataSource = _viewModel.Pushings;
        }
    }
}
