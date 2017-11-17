using System.Windows.Forms;

namespace QvaDev.Duplicat
{
    public static class Extensions
    {
        //public static SyncBindingSource<T> ToDataSource<T>(this ObservableCollection<T> list)
        //{
        //    return new SyncBindingSource<T>(list);
        //}

        public static void AddBinding(this Control control, string propertyName, object dataSource, string dataMember, bool inverse = false, bool oneWay = false)
        {
            Binding binding;
            if (inverse)
            {
                binding = new Binding(propertyName, dataSource, dataMember);
                binding.Format += (s, e) => e.Value = !(bool) e.Value;
            }
            else binding = new Binding(propertyName, dataSource, dataMember);
            control.DataBindings.Add(binding);
        }
    }
}
