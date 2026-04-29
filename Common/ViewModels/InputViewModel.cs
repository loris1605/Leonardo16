using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;
using ViewModels.BindableObjects;

namespace ViewModels
{
      

    public abstract partial class InputViewModel<TMap> : BaseViewModel where TMap : BindableMap, new()
    {
        public Interaction<Unit, Unit> EscFocus { get; } = new();

        public InputViewModel() : base(null)
        {
            this.WhenAnyValue(x => x.IsLoading, x => x._isClosing)
                .Select(states => !states.Item1 && !states.Item2)
                .Subscribe(x => FieldsEnabled = x);
        }


        private TMap bindingt = new();
        public TMap BindingT
        {
            get => bindingt;
            set => this.RaiseAndSetIfChanged(ref bindingt, value);
        }

        private bool fieldsenabled;
        public bool FieldsEnabled
        {
            get => fieldsenabled;
            set => this.RaiseAndSetIfChanged(ref fieldsenabled, value);
        }

        private bool fieldenabled;
        public bool FieldEnabled
        {
            get => fieldenabled;
            set => this.RaiseAndSetIfChanged(ref fieldenabled, value);
        }

        private bool fieldsvisibile;
        public bool FieldsVisibile
        {
            get => fieldsvisibile;
            set => this.RaiseAndSetIfChanged(ref fieldsvisibile, value);
        }

        private bool fieldvisibile = true;
        public bool FieldVisibile
        {
            get => fieldvisibile;
            set => this.RaiseAndSetIfChanged(ref fieldvisibile, value);
        }

        private string titolo = string.Empty;
        public string Titolo
        {
            get => titolo;
            set => this.RaiseAndSetIfChanged(ref titolo, value);
        }

        private string titolo1 = string.Empty;
        public string Titolo1
        {
            get => titolo1;
            set => this.RaiseAndSetIfChanged(ref titolo1, value);
        }

        private string infolabel = string.Empty;
        public string InfoLabel
        {
            get => infolabel;
            set => this.RaiseAndSetIfChanged(ref infolabel, value);
        }
    }
}
