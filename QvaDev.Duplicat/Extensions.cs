using System.Collections.ObjectModel;

namespace QvaDev.Duplicat
{
    public static class Extensions
    {
        public static SyncBindingSource<T> ToDataSource<T>(this ObservableCollection<T> list)
        {
            return new SyncBindingSource<T>(list);
        }
    }
}
