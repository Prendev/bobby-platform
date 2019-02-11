using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;

namespace TradeSystem.Duplicat
{
    public class SyncBindingSource<T> : BindingSource
    {
        private readonly SynchronizationContext _syncContext;
        public SyncBindingSource(ObservableCollection<T> list)
        {
            _syncContext = SynchronizationContext.Current;
            DataSource = new BindingList<T>(list);
        }
        protected override void OnListChanged(ListChangedEventArgs e)
        {
            if (_syncContext != null)
                _syncContext.Send(_ => base.OnListChanged(e), null);
            else
                base.OnListChanged(e);
        }
    }
}
