using QvaDev.Data;

namespace QvaDev.Duplicat.ViewModel
{
    public interface ICommand
    {
        void Execute(DuplicatContext duplicatContext, DuplicatViewModel viewModel, object parameter = null);
    }
}
