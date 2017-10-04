using System.ComponentModel;
using System.Data.Entity;
using System.Runtime.CompilerServices;
using Autofac;
using QvaDev.Data;
using QvaDev.Data.Models;
using QvaDev.Duplicat.Annotations;
using IContainer = Autofac.IContainer;

namespace QvaDev.Duplicat.ViewModel
{
    public class ViewModel : INotifyPropertyChanged
    {
        public enum States
        {
            Disconnect,
            Connect,
            Copy
        }

        private States _state;
        private readonly DuplicatContext _duplicatContext;
        
        private readonly IComponentContext _componentContext;
        public event PropertyChangedEventHandler PropertyChanged;

        public BindingList<MetaTraderPlatform> MetaTraderPlatforms { get; }
        public BindingList<CTraderPlatform> CTraderPlatforms { get; }
        public BindingList<MetaTraderAccount> MetaTraderAccounts { get; }
        public BindingList<CTraderAccount> CTraderAccounts { get; }
        public BindingList<Profile> Profiles { get; }
        public BindingList<Group> Groups { get; }

        public bool IsDisconnect { get => _state == States.Disconnect; set { if (value) _state = States.Disconnect; } }
        public bool IsConnect { get => _state == States.Connect; set { if (value) _state = States.Connect; } }
        public bool IsCopy { get => _state == States.Copy; set { if (value) _state = States.Copy; } }
        public bool IsConfigReadonly => _state != States.Disconnect;

        public ViewModel(
            IComponentContext componentContext,
            DuplicatContext duplicatContext)
        {
            _componentContext = componentContext;
            _duplicatContext = duplicatContext;

            _duplicatContext.MetaTraderPlatforms.Load();
            _duplicatContext.CTraderPlatforms.Load();
            _duplicatContext.MetaTraderAccounts.Load();
            _duplicatContext.CTraderAccounts.Load();
            _duplicatContext.Profiles.Load();
            _duplicatContext.Groups.Load();

            MetaTraderPlatforms = _duplicatContext.MetaTraderPlatforms.Local.ToBindingList();
            CTraderPlatforms = _duplicatContext.CTraderPlatforms.Local.ToBindingList();
            MetaTraderAccounts = _duplicatContext.MetaTraderAccounts.Local.ToBindingList();
            CTraderAccounts = _duplicatContext.CTraderAccounts.Local.ToBindingList();
            Profiles = _duplicatContext.Profiles.Local.ToBindingList();
            Groups = _duplicatContext.Groups.Local.ToBindingList();

            IsDisconnect = true;
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
