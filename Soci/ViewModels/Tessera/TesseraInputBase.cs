using Common.InterViewModels;
using ReactiveUI;
using SysNet;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using ViewModels.BindableObjects;

namespace ViewModels
{
    public partial class TesseraInputBase : InputViewModel<PersonMap>
    {
        string Cognome => BindingT is null ? "" : BindingT.Cognome.Trim();
        string Nome => BindingT is null ? "" : BindingT.Nome.Trim();
        string NumeroSocio => BindingT is null ? string.Empty : BindingT.NumeroSocio;
        int CodiceSocio => BindingT is null ? 0 : BindingT.CodiceSocio;
        int CodicePerson => BindingT is null ? 0 : BindingT.Id;

        protected string GetNumeroTessera => BindingT is null ? "" : BindingT.NumeroTessera;
        protected string GetNumeroSocio => NumeroSocio;
        protected int GetCodiceSocio => CodiceSocio;
        protected string GetNomeCognome => Nome + " " + Cognome;
        protected int GetCodicePerson => CodicePerson;

        protected ISociScreen _host;

        protected int _idDaModificare;
        protected int _idRitorno;

        protected void ResetNumeroTessera() => BindingT.NumeroTessera = string.Empty;

        public Interaction<Unit, Unit> NumeroTesseraFocus { get; } = new();
        
        public TesseraInputBase() : base()
        {
            
        }

        public void SetHost(ISociScreen host)
        {
            _host = host;
        }

        public void SetIdDaModificare(int id)
        {
            _idDaModificare = id;
        }

        public void SetIdRitorno(int id)
        {
            _idRitorno = id;
        }

        protected async override Task OnSaving() { await Task.CompletedTask; }
        protected async override Task OnLoading() { await Task.CompletedTask; }
        
        public async Task OnNumeroTesseraFocus()
        {
            // Fondamentale: aspetta un attimo che la View sia "viva" e l'handler registrato
            await Task.Delay(200);
            await SetFocus(NumeroTesseraFocus);
        }

        protected async override Task OnEsc()
        {
            if (_isClosing) return; // Protezione contro il multi-ESC

            if (_host is ISociScreen host)
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
                // 1. PROTEZIONE CRITICA: 
                // Se il primo click ha già svuotato lo stack, il secondo click 
                // deve uscire subito senza fare nulla.
                if (_host.InputRouter.NavigationStack.Count == 0)
                {
                    return;
                }

                // 2. Impostiamo IsLoading per disabilitare la UI
                _isClosing = true;

                try
                {
                    // 3. Eseguiamo il back solo perché abbiamo verificato che il Count > 0
                    await _host.InputRouter.NavigateBack.Execute();

                    // 4. Pulizia finale
                    _host.InputRouter.NavigationStack.Clear();
                    _host.AggiornaGridByInt(value);
                    _host.GroupEnabled = true;
                }
                catch (Exception ex)
                {
                    _isClosing = false;
                    Debug.WriteLine($"Errore durante la navigazione: {ex.Message}");
                }
            }
        }
    }
    
}
