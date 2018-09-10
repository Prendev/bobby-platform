using QvaDev.Duplicat.ViewModel;

namespace QvaDev.Duplicat.Views
{
    public interface IMvvmUserControl
    {
        void InitView(DuplicatViewModel viewModel);
        void AttachDataSources();
    }
}
