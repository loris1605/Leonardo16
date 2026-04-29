using Common.InterViewModels;
using ReactiveUI;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using ViewModels.BindableObjects;

namespace ViewModels
{
    public partial class PostazioneInputBase : InputViewModel<PostazioneMap>
    {

        protected string Name => BindingT?.NomePostazione?.Trim() ?? string.Empty;
        int CodicePostazione => BindingT?.Id ?? 0;
        protected int GetCodicePostazione => CodicePostazione;

        protected bool IsNameEmpty => string.IsNullOrWhiteSpace(Name);
        protected bool CheckLess2Name => Name.Length < 2;

        public Interaction<Unit, Unit> NomeFocus { get; } = new();

        protected IConfigurazioneScreen _host;
        protected int _idDaModificare;


        protected async override Task OnSaving() { await Task.CompletedTask; }
        protected async override Task OnLoading() => await Task.CompletedTask;

        public PostazioneInputBase() : base()
        {
            this.WhenActivated(d =>
            {

                // Logica IsCassa: aggiorna RientroVisibile quando cambia il tipo postazione
                this.WhenAnyValue(
                    x => x.BindingT,
                    x => x.BindingT.CodiceTipoPostazione,
                    (bt, codice) => bt is not null && codice == 2) // Questa è la tua logica IsCassa
                .Subscribe(isCassa =>
                {
                    // Se è una cassa, il rientro è visibile (o invisibile, a seconda della tua logica)
                    RientroVisibile = isCassa;
                })
                .DisposeWith(d);


            });
        }

        public void SetHost(IConfigurazioneScreen host)
        {
            _host = host;
        }

        public void SetIdDaModificare(int id)
        {
            _idDaModificare = id;
        }

        protected async Task<bool> ValidaDati()
        {
            if (IsNameEmpty)
            {
                InfoLabel = "Inserire il nome della posizione";
                await SetFocus(NomeFocus);
                return false;
            }

            if (CheckLess2Name)
            {
                InfoLabel = "Formato Nome Postazione non valido";
                await SetFocus(NomeFocus);
                return false;
            }

            InfoLabel = ""; // Pulisce eventuali errori precedenti
            return true;
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
    }

    public partial class PostazioneInputBase
    {
  
        private bool _rientroVisibile = true;
        public bool RientroVisibile
        {
            get => _rientroVisibile;
            set => this.RaiseAndSetIfChanged(ref _rientroVisibile, value);
        }

        private IList<TipoPostazioneMap> tipoPostazioneMaps = [];
        public IList<TipoPostazioneMap> TipoPostDataSource
        {
            get => tipoPostazioneMaps;
            set => this.RaiseAndSetIfChanged(ref tipoPostazioneMaps, value);
        }

        private IList<TipoRientroMap> _tipoRientroDataSource = [];
        public IList<TipoRientroMap> TipoRientroDataSource
        {
            get => _tipoRientroDataSource;
            set => this.RaiseAndSetIfChanged(ref _tipoRientroDataSource, value);
        }

        
        
    }
}
