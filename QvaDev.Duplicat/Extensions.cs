using System;
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
            var binding = new Binding(propertyName, dataSource, dataMember);
            if (inverse) binding.Format += (s, e) => e.Value = !(bool)e.Value;
            control.DataBindings.Add(binding);
        }

        public static void AddBinding<T>(this Control control, string propertyName, object dataSource, string dataMember, Func<T, bool> format)
        {
            var binding = new Binding(propertyName, dataSource, dataMember);
            binding.Format += (s, e) => e.Value = format((T)e.Value);
            control.DataBindings.Add(binding);
		}

		public static void AddBinding<T, TResult>(this Control control, string propertyName, object dataSource, string dataMember, Func<T, TResult> format)
		{
			var binding = new Binding(propertyName, dataSource, dataMember);
			binding.Format += (s, e) => e.Value = format((T)e.Value);
			control.DataBindings.Add(binding);
		}
	}
}
