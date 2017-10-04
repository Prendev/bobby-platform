using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using Autofac;
using QvaDev.Data;
using QvaDev.Data.Models;
using QvaDev.Duplicat.Annotations;

namespace QvaDev.Duplicat.ViewModel
{

    public class DuplicatViewModel : INotifyPropertyChanged
    {
        public enum States
        {
            Disconnect,
            Connect,
            Copy
        }

        public delegate void ProfileChangedEventHandler();

        private States _state;
        private DuplicatContext _duplicatContext;
        
        private readonly IComponentContext _componentContext;

        public BindingList<MetaTraderPlatform> MetaTraderPlatforms { get; private set; }
        public BindingList<CTraderPlatform> CTraderPlatforms { get; private set; }
        public BindingList<MetaTraderAccount> MetaTraderAccounts { get; private set; }
        public BindingList<CTraderAccount> CTraderAccounts { get; private set; }
        public BindingList<Profile> Profiles { get; private set; }
        public BindingList<Group> Groups { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public event ProfileChangedEventHandler ProfileChanged;

        private int _selectedProfileId;
        public int SelectedProfileId
        {
            get => _selectedProfileId;
            set { _selectedProfileId = value; LoadDataContext(); }
        }


        public bool IsDisconnect { get => _state == States.Disconnect; set { if (value) _state = States.Disconnect; } }
        public bool IsConnect { get => _state == States.Connect; set { if (value) _state = States.Connect; } }
        public bool IsCopy { get => _state == States.Copy; set { if (value) _state = States.Copy; } }
        public bool IsConfigReadonly => _state != States.Disconnect;

        public DuplicatViewModel(
            IComponentContext componentContext)
        {
            _componentContext = componentContext;

            IsDisconnect = true;

            LoadDataContext();
        }

        private void LoadDataContext()
        {
            _duplicatContext?.Dispose();
            _duplicatContext = new DuplicatContext();

            _duplicatContext.MetaTraderPlatforms.Load();
            _duplicatContext.CTraderPlatforms.Load();
            _duplicatContext.MetaTraderAccounts.Load();
            _duplicatContext.CTraderAccounts.Load();
            _duplicatContext.Profiles.Load();
            _duplicatContext.Groups.Where(g => g.ProfileId == SelectedProfileId).Load();

            MetaTraderPlatforms = _duplicatContext.MetaTraderPlatforms.Local.ToBindingList();
            CTraderPlatforms = _duplicatContext.CTraderPlatforms.Local.ToBindingList();
            MetaTraderAccounts = _duplicatContext.MetaTraderAccounts.Local.ToBindingList();
            CTraderAccounts = _duplicatContext.CTraderAccounts.Local.ToBindingList();
            Profiles = _duplicatContext.Profiles.Local.ToBindingList();
            Groups = _duplicatContext.Groups.Local.ToBindingList();

            ProfileChanged?.Invoke();
        }

        public void Execute<TCommand>(object parameter = null) where TCommand : ICommand
        {
            var command = _componentContext.Resolve<TCommand>();
            command.Execute(parameter);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
