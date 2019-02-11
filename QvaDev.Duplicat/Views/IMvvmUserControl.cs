using TradeSystem.Duplicat.ViewModel;

namespace TradeSystem.Duplicat.Views
{
    public interface IMvvmUserControl
    {
        void InitView(DuplicatViewModel viewModel);
        void AttachDataSources();
    }
}
