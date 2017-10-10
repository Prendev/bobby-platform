﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using QvaDev.Common;
using QvaDev.Data;
using QvaDev.Data.Models;
using QvaDev.Orchestration;

namespace QvaDev.Duplicat.ViewModel
{
    public class DuplicatViewModel : BaseViewModel
    {
        private enum States
        {
            Disconnect,
            Connect,
            Copy
        }

        public delegate void ProfileChangedEventHandler();
        
        private DuplicatContext _duplicatContext;
        private readonly IOrchestrator _orchestrator;

        public ObservableCollection<MetaTraderPlatform> MtPlatforms { get; private set;  }
        public ObservableCollection<CTraderPlatform> CtPlatforms { get; private set; }
        public ObservableCollection<MetaTraderAccount> MtAccounts { get; private set; }
        public ObservableCollection<CTraderAccount> CtAccounts { get; private set; }
        public ObservableCollection<Profile> Profiles { get; private set; }
        public ObservableCollection<Group> Groups { get; private set; }
        public ObservableCollection<Master> Masters { get; private set; }
        public ObservableCollection<Slave> Slaves { get; private set; }
        public ObservableCollection<SymbolMapping> SymbolMappings { get; private set; }
        public ObservableCollection<Copier> Copiers { get; private set; }

        public event ProfileChangedEventHandler ProfileChanged;
        
        private States State { get => Get<States>(); set => Set(value); }
        public bool IsDisconnect { get => State == States.Disconnect; set => State = value ? States.Disconnect : State; }
        public bool IsConnect { get => State == States.Connect; set => State = value ? States.Connect : State; }
        public bool IsCopy { get => State == States.Copy; set => State = value ? States.Copy : State; }
        public bool IsConfigReadonly { get => Get<bool>(); set => Set(value); }
        public bool IsLoading { get => Get<bool>(); set => Set(value); }

        public int SelectedProfileId { get => Get<int>(); set => Set(value); }
        public int SelectedSlaveId { get => Get<int>(); set => Set(value); }

        public DuplicatViewModel(
            IOrchestrator orchestrator)
        {
            _orchestrator = orchestrator;
            PropertyChanged += DuplicatViewModel_PropertyChanged;
            IsDisconnect = true;
            LoadDataContext();
        }

        public void SaveCommand()
        {
            _duplicatContext.SaveChanges();
        }

        public void LoadProfileCommand(Profile profile)
        {
            SelectedProfileId = profile?.Id ?? 0;
            LoadDataContext();
            ProfileChanged?.Invoke();
        }

        public void ShowSelectedSlaveCommand(Slave slave)
        {
            SelectedSlaveId = slave?.Id ?? 0;
            foreach (var e in SymbolMappings) e.IsFiltered = e.SlaveId != SelectedSlaveId;
            foreach (var e in Copiers) e.IsFiltered = e.SlaveId != SelectedSlaveId;
        }

        private void DuplicatViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(State)) StateChangeCommand();
        }

        private void StateChangeCommand()
        {
            IsLoading = true;
            IsConfigReadonly = true;
            if (State == States.Disconnect) _orchestrator.Disconnect(_duplicatContext)
                .ContinueWith(prevTask => { prevTask.Wait(); IsLoading = false; IsConfigReadonly = false; });
            else if (State == States.Connect) _orchestrator.Connect(_duplicatContext)
                .ContinueWith(prevTask => { prevTask.Wait(); IsLoading = false; IsConfigReadonly = true; });
            else if (State == States.Copy) _orchestrator.StartCopiers(_duplicatContext)
                .ContinueWith(prevTask => { prevTask.Wait(); IsLoading = false; IsConfigReadonly = true; });
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
            _duplicatContext.Groups.Where(e => e.ProfileId == SelectedProfileId)
                .Include(e => e.Masters).Load();
            _duplicatContext.Masters.Where(e => e.Group.ProfileId == SelectedProfileId)
                .Include(e => e.Slaves).Load();
            _duplicatContext.Slaves.Where(e => e.Master.Group.ProfileId == SelectedProfileId)
                .Include(e => e.Copiers).Include(e => e.SymbolMappings).Load();

            MtPlatforms = _duplicatContext.MetaTraderPlatforms.Local;
            CtPlatforms = _duplicatContext.CTraderPlatforms.Local;
            MtAccounts = _duplicatContext.MetaTraderAccounts.Local;
            CtAccounts = _duplicatContext.CTraderAccounts.Local;
            Profiles = _duplicatContext.Profiles.Local;
            Groups = _duplicatContext.Groups.Local;
            Masters = _duplicatContext.Masters.Local;
            Slaves = _duplicatContext.Slaves.Local;
            SymbolMappings = _duplicatContext.SymbolMappings.Local;
            Copiers = _duplicatContext.Copiers.Local;

            foreach (var e in SymbolMappings) e.IsFiltered = e.SlaveId != SelectedSlaveId;
            foreach (var e in Copiers) e.IsFiltered = e.SlaveId != SelectedSlaveId;
        }
    }
}
