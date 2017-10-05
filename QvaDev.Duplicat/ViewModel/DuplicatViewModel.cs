using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using Autofac;
using QvaDev.Data;
using QvaDev.Data.Models;

namespace QvaDev.Duplicat.ViewModel
{
    public class DuplicatViewModel : BaseViewModel
    {
        public delegate void ProfileChangedEventHandler();

        private readonly IComponentContext _componentContext;
        private DuplicatContext _duplicatContext;

        public ObservableCollection<MetaTraderPlatform> MtPlatforms { get; private set;  }
        public ObservableCollection<CTraderPlatform> CtPlatforms { get; private set; }
        public ObservableCollection<MetaTraderAccount> MtAccounts { get; private set; }
        public ObservableCollection<CTraderAccount> CtAccounts { get; private set; }
        public ObservableCollection<Profile> Profiles { get; private set; }
        public ObservableCollection<Group> Groups { get; private set; }
        public ObservableCollection<Master> Masters { get; private set; }
        public ObservableCollection<Slave> Slaves { get; private set; }

        public event ProfileChangedEventHandler ProfileChanged;
        
        public int SelectedProfileId { get => Get<int>(); set => Set(value); }
        public bool IsDisconnect { get => Get<bool>(); set => Set(value); }
        public bool IsConnect { get => Get<bool>(); set => Set(value); }
        public bool IsCopy { get => Get<bool>(); set => Set(value); }
        public bool IsConfigReadonly { get => Get<bool>(); set => Set(value); }
        public bool IsConfigEditable { get => Get<bool>(); set => Set(value); }

        public DuplicatViewModel(
            IComponentContext componentContext)
        {
            _componentContext = componentContext;
            PropertyChanged += DuplicatViewModel_PropertyChanged;
            IsDisconnect = true;
            LoadDataContext();
        }

        private void DuplicatViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IsDisconnect) || e.PropertyName == nameof(IsConnect) ||
                e.PropertyName == nameof(IsCopy))
            {
                IsConfigReadonly = !IsDisconnect;
                IsConfigEditable = IsDisconnect;
            }
            else if (e.PropertyName == nameof(SelectedProfileId))
            {
                LoadDataContext();
                ProfileChanged?.Invoke();
            }
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
            _duplicatContext.Masters.Where(g => g.Group.ProfileId == SelectedProfileId).Load();
            _duplicatContext.Slaves.Where(g => g.Master.Group.ProfileId == SelectedProfileId).Load();

            MtPlatforms = _duplicatContext.MetaTraderPlatforms.Local;
            CtPlatforms = _duplicatContext.CTraderPlatforms.Local;
            MtAccounts = _duplicatContext.MetaTraderAccounts.Local;
            CtAccounts = _duplicatContext.CTraderAccounts.Local;
            Profiles = _duplicatContext.Profiles.Local;
            Groups = _duplicatContext.Groups.Local;
            Masters = _duplicatContext.Masters.Local;
            Slaves = _duplicatContext.Slaves.Local;
        }

        public void Execute<TCommand>(object parameter = null) where TCommand : ICommand
        {
            var command = _componentContext.Resolve<TCommand>();
            command.Execute(_duplicatContext, this, parameter);
        }
    }
}
