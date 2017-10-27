using QvaDev.Duplicat.ViewModel;

namespace QvaDev.Duplicat.Views
{
    public interface ITabUserControl
    {
        void InitView(DuplicatViewModel viewModel);
        void AttachDataSources();
    }
}
