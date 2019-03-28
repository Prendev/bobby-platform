using System;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using TradeSystem.Common.Integration;
using TradeSystem.Data.Models;
using TradeSystem.Duplicat.ViewModel;

namespace TradeSystem.Duplicat.Views
{
	public partial class SpoofingUserControl : UserControl, IMvvmUserControl
	{
		private DuplicatViewModel _viewModel;

		public SpoofingUserControl()
		{
			InitializeComponent();
		}

		public void InitView(DuplicatViewModel viewModel)
		{
			_viewModel = viewModel;

			gbControl.AddBinding("Enabled", _viewModel, nameof(_viewModel.IsLoading), true);
			gbFlow.AddBinding("Enabled", _viewModel, nameof(_viewModel.IsSpoofingEnabled));
			dgvSpoofings.AddBinding("AllowUserToAddRows", _viewModel, nameof(_viewModel.IsConfigReadonly), true);
			dgvSpoofings.AddBinding("AllowUserToDeleteRows", _viewModel, nameof(_viewModel.IsConfigReadonly), true);
			btnStartCopiers.AddBinding("Enabled", _viewModel, nameof(_viewModel.AreCopiersStarted), true);
			btnStopCopiers.AddBinding("Enabled", _viewModel, nameof(_viewModel.AreCopiersStarted));

			gbSpoofings.AddBinding<Spoofing, string>("Text", _viewModel, nameof(_viewModel.SelectedSpoofing),
				s => $"Spoofings (use double-click) - {s?.ToString() ?? "Save before load!!!"}");

			btnOpenSpoofUp.AddBinding<DuplicatViewModel.SpoofingStates>("Enabled", _viewModel,
				nameof(_viewModel.SpoofingState), p => p == DuplicatViewModel.SpoofingStates.NotRunning);
			btnOpenSpoofDown.AddBinding<DuplicatViewModel.SpoofingStates>("Enabled", _viewModel,
				nameof(_viewModel.SpoofingState), p => p == DuplicatViewModel.SpoofingStates.NotRunning);
			btnOpenBetaRush.AddBinding<DuplicatViewModel.SpoofingStates>("Enabled", _viewModel,
				nameof(_viewModel.SpoofingState), p => p == DuplicatViewModel.SpoofingStates.BeforeOpeningBeta);
			btnOpenBetaRushMore.AddBinding<DuplicatViewModel.SpoofingStates>("Enabled", _viewModel,
				nameof(_viewModel.SpoofingState), p => p == DuplicatViewModel.SpoofingStates.AfterOpeningBeta);
			btnOpenAlphaRush.AddBinding<DuplicatViewModel.SpoofingStates>("Enabled", _viewModel,
				nameof(_viewModel.SpoofingState), p => p == DuplicatViewModel.SpoofingStates.BeforeOpeningAlpha);
			btnOpenAlphaRushMore.AddBinding<DuplicatViewModel.SpoofingStates>("Enabled", _viewModel,
				nameof(_viewModel.SpoofingState), p => p == DuplicatViewModel.SpoofingStates.AfterOpeningAlpha);

			btnCloseSpoofUp.AddBinding<DuplicatViewModel.SpoofingStates>("Enabled", _viewModel,
				nameof(_viewModel.SpoofingState), p => p == DuplicatViewModel.SpoofingStates.BeforeClosing);
			btnCloseSpoofDown.AddBinding<DuplicatViewModel.SpoofingStates>("Enabled", _viewModel,
				nameof(_viewModel.SpoofingState), p => p == DuplicatViewModel.SpoofingStates.BeforeClosing);
			btnCloseFirstRush.AddBinding<DuplicatViewModel.SpoofingStates>("Enabled", _viewModel,
				nameof(_viewModel.SpoofingState), p => p == DuplicatViewModel.SpoofingStates.BeforeClosingFirst);
			btnCloseFirstRushMore.AddBinding<DuplicatViewModel.SpoofingStates>("Enabled", _viewModel,
				nameof(_viewModel.SpoofingState), p => p == DuplicatViewModel.SpoofingStates.AfterClosingFirst);
			btnCloseSecondRush.AddBinding<DuplicatViewModel.SpoofingStates>("Enabled", _viewModel,
				nameof(_viewModel.SpoofingState), p => p == DuplicatViewModel.SpoofingStates.BeforeClosingSecond);
			btnCloseSecondRushMore.AddBinding<DuplicatViewModel.SpoofingStates>("Enabled", _viewModel,
				nameof(_viewModel.SpoofingState), p => p == DuplicatViewModel.SpoofingStates.AfterClosingSecond);

			dgvSpoofings.DefaultValuesNeeded += (s, e) => e.Row.Cells["ProfileId"].Value = _viewModel.SelectedProfile.Id;

			btnStartCopiers.Click += (s, e) => { _viewModel.StartCopiersCommand(); };
			btnStopCopiers.Click += (s, e) => { _viewModel.StopCopiersCommand(); };
			btnSubscribeFeed.Click += (s, e) => { _viewModel.SpoofingFeedSubscribe(dgvSpoofings.GetSelectedItem<Spoofing>()); };
			btnBuyFutures.Click += (s, e) =>
			{
				var pushing = dgvSpoofings.GetSelectedItem<Spoofing>();
				_viewModel.SendOrderCommand(pushing.TradeAccount, Sides.Buy, pushing.TradeSymbol, nudFuturesContractSize.Value);
			};
			btnSellFutures.Click += (s, e) =>
			{
				var pushing = dgvSpoofings.GetSelectedItem<Spoofing>();
				_viewModel.SendOrderCommand(pushing.TradeAccount, Sides.Sell, pushing.TradeSymbol, nudFuturesContractSize.Value);
			};

			btnOpenSpoofUp.Click += (s, e) => { _viewModel.SpoofingOpenCommand(dgvSpoofings.GetSelectedItem<Spoofing>(), Sides.Sell); };
			btnOpenSpoofDown.Click += (s, e) => { _viewModel.SpoofingOpenCommand(dgvSpoofings.GetSelectedItem<Spoofing>(), Sides.Buy); };
			btnOpenBetaRush.Click += (s, e) => { _viewModel.SpoofingPanicCommand(dgvSpoofings.GetSelectedItem<Spoofing>()); };
			btnOpenBetaRushMore.Click += (s, e) => { _viewModel.SpoofingPanicCommand(dgvSpoofings.GetSelectedItem<Spoofing>()); };
			btnOpenAlphaRush.Click += (s, e) => { _viewModel.SpoofingPanicCommand(dgvSpoofings.GetSelectedItem<Spoofing>()); };
			btnOpenAlphaRushMore.Click += (s, e) => { _viewModel.SpoofingPanicCommand(dgvSpoofings.GetSelectedItem<Spoofing>()); };

			btnCloseSpoofUp.Click += (s, e) => { _viewModel.SpoofingCloseCommand(dgvSpoofings.GetSelectedItem<Spoofing>(), Sides.Buy); };
			btnCloseSpoofDown.Click += (s, e) => { _viewModel.SpoofingCloseCommand(dgvSpoofings.GetSelectedItem<Spoofing>(), Sides.Sell); };
			btnCloseFirstRush.Click += (s, e) => { _viewModel.SpoofingPanicCommand(dgvSpoofings.GetSelectedItem<Spoofing>()); };
			btnCloseFirstRushMore.Click += (s, e) => { _viewModel.SpoofingPanicCommand(dgvSpoofings.GetSelectedItem<Spoofing>()); };
			btnCloseSecondRush.Click += (s, e) => { _viewModel.SpoofingPanicCommand(dgvSpoofings.GetSelectedItem<Spoofing>()); };
			btnCloseSecondRushMore.Click += (s, e) => { _viewModel.SpoofingPanicCommand(dgvSpoofings.GetSelectedItem<Spoofing>()); };

			btnReset.Click += (s, e) => { _viewModel.SpoofingResetCommand(); };

			dgvSpoofings.RowDoubleClick += Load_Click;
		}

		private void Load_Click(object sender, EventArgs e)
		{
			if (_viewModel.IsConfigReadonly) return;
			_viewModel.ShowSpoofingCommand(dgvSpoofings.GetSelectedItem<Spoofing>());
		}

		public void AttachDataSources()
		{
			dgvSpoofings.AddComboBoxColumn(_viewModel.Accounts, "FeedAccount");
			dgvSpoofings.AddComboBoxColumn(_viewModel.Accounts, "TradeAccount");
			dgvSpoofings.AddComboBoxColumn(_viewModel.Accounts, "AlphaMaster");
			dgvSpoofings.AddComboBoxColumn(_viewModel.Accounts, "BetaMaster");
			dgvSpoofings.AddComboBoxColumn(_viewModel.Accounts, "HedgeAccount");
			dgvSpoofings.DataSource = _viewModel.Spoofings.ToBindingList();
		}
	}
}
