using Common.InterViewModels;
using ReactiveUI;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using ViewModels.BindableObjects;

namespace ViewModels
{
    public partial class SettoreInputBase : InputViewModel<SettoreMap>
    {

        protected async override Task OnSaving() { await Task.CompletedTask; }
        protected async override Task OnLoading() => await Task.CompletedTask;

        public Interaction<Unit, Unit> NomeFocus { get; } = new();
        public Interaction<Unit, Unit> LabelFocus { get; } = new();

        protected IConfigurazioneScreen _host;

        protected int _idDaModificare;

        public SettoreInputBase() : base()
        {

        }

        public void SetHost(IConfigurazioneScreen host)
        {
            _host = host;
        }

        public void SetIdDaModificare(int id)
        {
            _idDaModificare = id;
        }

        protected async override Task OnEsc()
        {
            if (_isClosing) return; // Protezione contro il multi-ESC

            if (_host is IConfigurazioneScreen host)
            {
                // Focus sul tasto Esci prima di chiudere
                await SetFocus(EscFocus, 0);
                _isClosing = true; // "Congeliamo" prima di uscire

                RxSchedulers.MainThreadScheduler.Schedule(() =>
                {
                    host.InputRouter.NavigationStack.Clear();
                    host.GroupEnabled = true;
                });
            }
        }

        protected async Task OnBack(int value = 0)
        {
            if (_host is not null)
            {
                if (_host.InputRouter.NavigationStack.Count == 0) return;

                _isClosing = true;
                try
                {
                    await _host.InputRouter.NavigateBack.Execute();
                    _host.InputRouter.NavigationStack.Clear();
                    _host.AggiornaGridByInt(value);
                    _host.GroupEnabled = true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Errore navigazione: {ex.Message}");
                    _isClosing = false;
                }
            }
        }

        protected string Name => BindingT?.NomeSettore?.Trim() ?? string.Empty;
        protected string Label => BindingT?.EtichettaSettore?.Trim() ?? string.Empty;
        protected int CodiceSettore => BindingT?.Id ?? 0;
        protected int GetCodiceSettore => CodiceSettore;

        protected bool IsNameEmpty => string.IsNullOrWhiteSpace(Name);
        protected bool CheckLess2Name => Name.Length < 2;

        protected bool IsLabelEmpty => string.IsNullOrWhiteSpace(Label);
        protected bool CheckLess2Label => Label.Length < 2;

        protected async Task<bool> ValidaDati()
        {
            if (IsNameEmpty)
            {
                InfoLabel = "Inserire il nome del settore";
                await SetFocus(NomeFocus);
                return false;
            }
            if (CheckLess2Name)
            {
                InfoLabel = "Formato Nome Settore non valido";
                await SetFocus(NomeFocus);
                return false;
            }
            if (IsLabelEmpty)
            {
                InfoLabel = "Inserire l'etichetta del settore";
                await SetFocus(LabelFocus);
                return false;
            }
            if (CheckLess2Label)
            {
                InfoLabel = "Formato Etichetta Settore non valido";
                await SetFocus(LabelFocus);
                return false;
            }
            InfoLabel = ""; // Pulisce eventuali errori precedenti
            return true;

        }
    }

    public partial class SettoreInputBase
    {

        
        private int _codiceTipoSettore = 0;
        public int CodiceTipoSettore
        {
            get => _codiceTipoSettore;
            set => this.RaiseAndSetIfChanged(ref _codiceTipoSettore, value);
        }

        private IList<TipoSettoreMap> tipoSettoreMaps = [];
        public IList<TipoSettoreMap> TipoSettDataSource
        {
            get => tipoSettoreMaps;
            set => this.RaiseAndSetIfChanged(ref tipoSettoreMaps, value);
        }


        

    }
}


