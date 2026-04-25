using ReactiveUI;

namespace ViewModels.BindableObjects
{
    public class BindableMap : ReactiveObject
    {
        protected int _codice;
        public int Id
        {
            get => _codice;
            set => this.RaiseAndSetIfChanged(ref _codice, value);
        }

        public virtual string Titolo { get; set; } = string.Empty;
        public virtual string Nome { get; set; } = string.Empty;
        public override string ToString() => Nome ?? string.Empty;
    }
}
