using QvaDev.Data;
using QvaDev.Data.Models;

namespace QvaDev.Duplicat.ViewModel
{
    public class LoadProfileCommand : ICommand
    {
        public void Execute(DuplicatContext duplicatContext, DuplicatViewModel viewModel, object parameter = null)
        {
            viewModel.SelectedProfileId = (parameter as Profile)?.Id ?? 0;
        }
    }
}
