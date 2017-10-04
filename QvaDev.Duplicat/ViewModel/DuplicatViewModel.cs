using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
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

        public List<Profile> SelectorProfiles
        {
            get => _selectorProfiles;
            set
            {
                if (value == _selectorProfiles) return;
                _selectorProfiles = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<MetaTraderPlatform> MtPlatforms { get; private set; }
        public ObservableCollection<CTraderPlatform> CtPlatforms { get; private set; }
        public ObservableCollection<MetaTraderAccount> MtAccounts { get; private set; }
        public ObservableCollection<CTraderAccount> CtAccounts { get; private set; }
        public ObservableCollection<Profile> Profiles { get; private set; }
        public ObservableCollection<Group> Groups { get; private set; }
        public ObservableCollection<Master> Masters { get; private set; }
        public ObservableCollection<Slave> Slaves { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public event ProfileChangedEventHandler ProfileChanged;

        private int _selectedProfileId;
        private List<Profile> _selectorProfiles;

        public int SelectedProfileId
        {
            get => _selectedProfileId;
            set
            {
                _selectedProfileId = value;
                LoadDataContext();
                ProfileChanged?.Invoke();
            }
        }


        public bool IsDisconnect { get => _state == States.Disconnect; set { if (value) _state = States.Disconnect; } }
        public bool IsConnect { get => _state == States.Connect; set { if (value) _state = States.Connect; } }
        public bool IsCopy { get => _state == States.Copy; set { if (value) _state = States.Copy; } }
        public bool IsConfigReadonly => _state != States.Disconnect;
        public bool IsConfigEditable => _state == States.Disconnect;

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

            SelectorProfiles = SelectorProfiles ?? new List<Profile>(_duplicatContext.Profiles.ToList());

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

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
