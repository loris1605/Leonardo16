using Common.InterViewModels;
using ReactiveUI;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using ViewModels.BindableObjects;

namespace ViewModels
{
    public partial class TariffaInputBase : InputViewModel<TariffaMap>
    {
        protected async override Task OnSaving() { await Task.CompletedTask; }
        protected async override Task OnLoading() => await Task.CompletedTask;

        public Interaction<Unit, Unit> NomeFocus { get; } = new();
        public Interaction<Unit, Unit> LabelFocus { get; } = new();

        protected IConfigurazioneScreen _host;

        protected int _idDaModificare;

        public string Name => BindingT.NomeTariffa.Trim() is null ? "" : BindingT.NomeTariffa.Trim();
        string Label => BindingT.EtichettaTariffa.Trim() is null ? "" : BindingT.EtichettaTariffa.Trim();
        protected int GetCodiceTariffa => BindingT is null ? 0 : BindingT.Id;

        protected bool IsNameEmpty => BindingT is not null && (Name == "");
        protected bool CheckLess2Name => Name.Length < 2;
        public bool IsLabelEmpty => BindingT is not null && (Label == "");
        public bool CheckLess2Label => Label.Length < 2;

        public TariffaInputBase() : base()
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
    }

    public partial class TariffaInputBase
    {
        protected async Task<bool> ValidaDati()
        {
            if (IsNameEmpty)
            {
                InfoLabel = "Inserire il nome della tariffa";
                await SetFocus(NomeFocus);
                return false;
            }
            if (CheckLess2Name)
            {
                InfoLabel = "Formato Nome Tariffa non valido";
                await SetFocus(NomeFocus);
                return false;
            }
            if (IsLabelEmpty)
            {
                InfoLabel = "Inserire l'etichetta della tariffa";
                await SetFocus(LabelFocus);
                return false;
            }
            if (CheckLess2Label)
            {
                InfoLabel = "Formato Etichetta Tariffa non valido";
                await SetFocus(LabelFocus);
                return false;
            }
            InfoLabel = ""; // Pulisce eventuali errori precedenti
            return true;

        }

        
        
        

    }
}
