using QvaDev.Data;

namespace QvaDev.Duplicat.ViewModel
{
    public class SaveCommand : ICommand
    {
        private readonly DuplicatContext _duplicatContext;

        public SaveCommand(
            DuplicatContext duplicatContext)
        {
            _duplicatContext = duplicatContext;
        }

        public void Execute(object parameter = null)
        {
            _duplicatContext.SaveChanges();
        }
    }
}
