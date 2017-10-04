using System.Linq;
using QvaDev.Data;

namespace QvaDev.Duplicat.ViewModel
{
    public class SaveCommand : ICommand
    {
        public void Execute(DuplicatContext duplicatContext, DuplicatViewModel viewModel, object parameter = null)
        {
            duplicatContext.SaveChanges();
            viewModel.SelectorProfiles = duplicatContext.Profiles.ToList();
        }
    }
}
