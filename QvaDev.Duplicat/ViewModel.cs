using System.ComponentModel;
using System.Runtime.CompilerServices;
using QvaDev.Duplicat.Annotations;

namespace QvaDev.Duplicat
{
    public class ViewModel : INotifyPropertyChanged
    {
        private States _state;
        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsDisconnect { get => _state == States.Disconnect; set { if (value) _state = States.Disconnect; } }
        public bool IsConnect { get => _state == States.Connect; set { if (value) _state = States.Connect; } }
        public bool IsCopy { get => _state == States.Copy; set { if (value) _state = States.Copy; } }
        public bool IsConfigReadonly => _state != States.Disconnect;

        public ViewModel()
        {
            IsDisconnect = true;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public enum States
        {
            Disconnect,
            Connect,
            Copy
        }
    }
}
