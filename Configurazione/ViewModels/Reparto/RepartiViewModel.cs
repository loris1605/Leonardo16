using Common.InterViewModels;
using DTO.Repository;
using ReactiveUI;
using ViewModels.BindableObjects;

namespace ViewModels
{
    public partial class RepartiViewModel : PostazioneInputBase, IRepartoViewModel
    {
        private IRepartoRepository Q;
        private IPostazioneRepository pQ;
        public RepartiViewModel(IPostazioneRepository pRepository, IRepartoRepository Repository) : base()
        {
            Q = Repository ?? throw new ArgumentNullException(nameof(Repository));
            pQ = pRepository ?? throw new ArgumentNullException(nameof(pRepository));
        }

        protected override void OnFinalDestruction()
        {
            Q = null;
            pQ = null;
            base.OnFinalDestruction();
        }

        protected override async Task OnLoading()
        {
            var postazione = await pQ.FirstPostazione(_idDaModificare, token);
            Titolo = $"Reparti per la postazione: {postazione?.NomePostazione ?? "Sconosciuto"}";
            var data = await Q.GetReparti(_idDaModificare, token);
            
            DataSource = data.Select(dto => new SettoreElencoMap(dto)).ToList();
            await SetFocus(EscFocus);
        }

        protected async override Task OnSaving()
        {
            if (DataSource == null) return;
            _isClosing = true;

            var dtoSource = DataSource.Select(p => p.ToDto()).ToList();

            try
            {
                InfoLabel = "Salvataggio reparti...";

                if (!await Q.SaveReparti(_idDaModificare, dtoSource, token))
                {
                    InfoLabel = "Errore Database: modifica reparti fallita";
                    await SetFocus(EscFocus);
                    return;
                }

                // Successo: ritorno protetto
                await OnBack(_idDaModificare);
            }
            catch (OperationCanceledException) { _isClosing = false; }
            catch (Exception ex)
            {
                _isClosing = false;
                InfoLabel = $"Errore: {ex.Message}";
                await SetFocus(EscFocus);
            }
        }

    }

    public partial class RepartiViewModel
    {
        #region DataSource

        private IList<SettoreElencoMap> _datasource = [];
        public IList<SettoreElencoMap> DataSource
        {
            get => _datasource;
            set => this.RaiseAndSetIfChanged(ref _datasource, value);
        }

        #endregion

        #region BindingT

        private SettoreElencoMap _bindingT;
        public new SettoreElencoMap BindingT
        {
            get => _bindingT;
            set => this.RaiseAndSetIfChanged(ref _bindingT, value);
        }

        #endregion


    }
}
