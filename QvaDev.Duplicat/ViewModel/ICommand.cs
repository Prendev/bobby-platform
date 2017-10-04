using QvaDev.Data;

namespace QvaDev.Duplicat.ViewModel
{
    public interface ICommand
    {
        void Execute(DuplicatContext duplicatContext, object parameter = null);
    }
}
