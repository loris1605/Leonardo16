using Common.InterViewModels;
using FluentIcons.Common.Internals;
using ReactiveUI;
using SysNet.Converters;
using System.Reactive;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using ViewModels.BindableObjects;

namespace ViewModels
{
    
    public partial  class PersonInputBase : InputViewModel<PersonMap>
    {
        protected int CodicePerson => BindingT is null ? 0 : BindingT.Id;
        protected int Natoil => BindingT is null ? 0 : BindingT.Natoil;

        protected int CodiceSocio => BindingT is null ? 0 : BindingT.CodiceSocio;
        protected int CodiceTessera => BindingT is null ? 0 : BindingT.CodiceTessera;
        protected string CodiceUnivoco => BindingT?.CodiceUnivoco?.Trim() ?? "";

        protected bool IsCognomeEmpty => string.IsNullOrWhiteSpace(BindingT?.Cognome);
        protected bool IsNomeEmpty => string.IsNullOrWhiteSpace(BindingT?.Nome);
        protected bool CheckLess2Surname => (BindingT?.Cognome?.Length ?? 0) < 2;
        protected bool CheckLess2FirstName => (BindingT?.Nome?.Length ?? 0) < 2;
        
        protected bool IsLegalAge => BindingT.Natoil.IsLegalAge();
        protected string GetNumeroTessera => BindingT?.NumeroTessera?.Trim() ?? "";
        protected string GetNumeroSocio => BindingT?.NumeroSocio?.Trim() ?? "";
        protected int GetCodicePerson => CodicePerson;

        protected string GetCognome => BindingT?.Cognome?.Trim() ?? "";
        protected string GetNome => BindingT?.Nome?.Trim() ?? "";

        protected ISociScreen _host;

        protected int _idDaModificare;
        protected int _idRitorno;

        public PersonInputBase() : base()
        {
            
            this.WhenActivated(d =>
            {
                
                this.WhenAnyValue(x => x.DataNascitaOffSet)
                    .Where(_ => BindingT != null)
                    .Subscribe(val => BindingT.Natoil = val.DateTimeOffsetToDateInt())
                    .DisposeWith(d);

                this.WhenAnyValue(x => x.BindingT.Natoil) // Monitora specificamente questa proprietà
                    .Where(natoil => natoil != default)   // O != 0, a seconda del tipo di Natoil
                    .Subscribe(natoil =>
                    {
                        // Qui 'natoil' è già il valore della proprietà specifica, non l'intero oggetto BindingT
                        this.DataNascitaOffSet = natoil.DateIntToDateTimeOffset();
                    })
                    .DisposeWith(d);

            });
            
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
        
        protected async Task<bool> ValidaDati()
        {
            if (IsCognomeEmpty)
            {
                InfoLabel = "Inserire il cognome del socio";
                await SetFocus(CognomeFocus);
                return false;
            }

            if (IsNomeEmpty)
            {
                InfoLabel = "Inserire il nome del socio";
                await SetFocus(NomeFocus);
                return false;
            }

            if (CheckLess2Surname || CheckLess2FirstName)
            {
                InfoLabel = "Formato nome o cognome non valido (min. 2 caratteri)";
                await SetFocus(CheckLess2Surname ? CognomeFocus : NomeFocus);
                return false;
            }

            if (!IsLegalAge)
            {
                InfoLabel = "Il socio deve essere maggiorenne";
                await SetFocus(NatoFocus);
                return false;
            }
        

            InfoLabel = ""; // Pulisce eventuali errori precedenti
            return true;
        }

        protected async override Task OnEsc()
        {
            if (_isClosing) return; // Protezione contro il multi-ESC

            if (_host is not null)
            {
                // Focus sul tasto Esci prima di chiudere
                await SetFocus(EscFocus, 0);
                _isClosing = true; // "Congeliamo" prima di uscire

                _inputEsc.OnNext(Unit.Default); // Notifica l'esterno che ESC è stato premuto
                
            }
        }

        protected async Task OnBack(int value = 0)
        {
            if (_host is not null)
            {
                
                if (_host.InputRouter.NavigationStack.Count == 0) return;

                _isClosing = true;

                _inputBack.OnNext(value); // Notifica l'esterno che Back è stato premuto con il valore specificato

                await Task.CompletedTask;
                
            }
        }

        
    }

    public partial class PersonInputBase
    {
        
        private DateTimeOffset? datanascitaoffset = new DateTimeOffset(DateTime.Now);
        public DateTimeOffset? DataNascitaOffSet
        {
            get => datanascitaoffset;
            set => this.RaiseAndSetIfChanged(ref datanascitaoffset, value);
        }

        
        private List<PersonMap> _datasource = [];
        public List<PersonMap> DataSource
        {
            get => _datasource;
            set => this.RaiseAndSetIfChanged(ref _datasource, value);
        }

        public Interaction<Unit, Unit> CognomeFocus { get; } = new();
        public Interaction<Unit, Unit> NomeFocus { get; } = new();
        public Interaction<Unit, Unit> NatoFocus { get; } = new();
        public Interaction<Unit, Unit> TesseraFocus { get; } = new();
        public Interaction<Unit, Unit> SocioFocus { get; } = new();

        // 1. Aggiungi questo Subject per notificare l'esterno
        private readonly Subject<Unit> _inputEsc = new();
        public IObservable<Unit> InputEsc => _inputEsc.AsObservable();

        private readonly Subject<int> _inputBack = new();
        public IObservable<int> InputBack => _inputBack.AsObservable();

        private readonly Subject<List<PersonMap>> _inputBackFiltered = new();
        public IObservable<List<PersonMap>> InputBackFiltered => _inputBackFiltered.AsObservable();


    }
}
