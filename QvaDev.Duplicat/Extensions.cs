using System.Windows.Forms;

namespace QvaDev.Duplicat
{
    public static class Extensions
    {
        //public static SyncBindingSource<T> ToDataSource<T>(this ObservableCollection<T> list)
        //{
        //    return new SyncBindingSource<T>(list);
        //}

        public static void AddBinding(this Control control, string propertyName, object dataSource, string dataMember, bool inverse = false)
        {
            if (inverse)
            {
                var inverseBinding = new Binding(propertyName, dataSource, dataMember);
                inverseBinding.Format += (s, e) => e.Value = !(bool)e.Value;
                control.DataBindings.Add(inverseBinding);
            }
            else control.DataBindings.Add(new Binding(propertyName, dataSource, dataMember));
        }
    }
}
