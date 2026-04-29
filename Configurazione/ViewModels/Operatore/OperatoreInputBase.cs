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
    public partial class OperatoreInputBase : InputViewModel<OperatoreMap>
    {
        protected int CodiceOperatore => BindingT?.Id ?? 0;

        public int GetCodiceOperatore => CodiceOperatore;

        public bool IsNicknameEmpty => string.IsNullOrWhiteSpace(BindingT?.NomeOperatore);
        public bool IsPasswordEmpty => string.IsNullOrWhiteSpace(BindingT?.Password);

        // Protezione per evitare NullReferenceException o errori di lunghezza su stringhe nulle
        public bool CheckLess2Nickname => (BindingT?.NomeOperatore?.Length ?? 0) < 2;
        public bool CheckLess2Password => (BindingT?.Password?.Length ?? 0) < 2;


        public Interaction<Unit, Unit> NomeFocus { get; } = new();
        public Interaction<Unit, Unit> PasswordFocus { get; } = new();

        protected IConfigurazioneScreen _host;

        protected int _idDaModificare;

        public OperatoreInputBase() : base()
        {

            this.WhenActivated(d =>
            {

                this.WhenAnyValue(x => x.BindingT.Abilitato)
                    .Select(val => val ? "Si" : "No") // Trasforma il bool in stringa
                    .Subscribe(text => AbilitatoText = text) // Assegna il risultato
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
            if (IsNicknameEmpty)
            {
                InfoLabel = "Inserire il nome dell'operatore";
                await SetFocus(NomeFocus);
                return false;
            }

            if (IsPasswordEmpty)
            {
                InfoLabel = "Inserire la password di accesso";
                await SetFocus(PasswordFocus);
                return false;
            }

            if (CheckLess2Nickname)
            {
                InfoLabel = "Formato nome non valido (min. 2 caratteri)";
                await SetFocus(NomeFocus);
                return false;
            }

            if (CheckLess2Password)
            {
                InfoLabel = "Formato password non valido (min. 2 caratteri)";
                await SetFocus(PasswordFocus);
                return false;
            }


            InfoLabel = ""; // Pulisce eventuali errori precedenti
            return true;
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
    }

    public partial class OperatoreInputBase
    {
  
        private string abilitatotext = string.Empty;
        public string AbilitatoText
        {
            get => abilitatotext;
            set => this.RaiseAndSetIfChanged(ref abilitatotext, value);
        }

        private bool nomeoperatoreenabled = true;
        public bool NomeOperatoreEnabled
        {
            get => nomeoperatoreenabled;
            set => this.RaiseAndSetIfChanged(ref nomeoperatoreenabled, value);
        }

        
       
        
    }
}
