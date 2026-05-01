using Common.InterViewModels;
using DTO.Repository;
using ReactiveUI;
using ViewModels.BindableObjects;

namespace ViewModels
{
    public partial class ListinoViewModel : SettoreInputBase, IListinoViewModel
    {
        private IListinoRepository Q;
        private ISettoreRepository sQ;

        public ListinoViewModel(ISettoreRepository settoreRepository, IListinoRepository listinoRepository) : base()
        {
            sQ = settoreRepository ?? throw new ArgumentNullException(nameof(settoreRepository));
            Q = listinoRepository ?? throw new ArgumentNullException(nameof(listinoRepository));
        }

        protected override void OnFinalDestruction()
        {
            Q = null;
            sQ = null;
            base.OnFinalDestruction();
        }

        protected override async Task OnLoading()
        {
            var settore = await sQ.FirstSettore(_idDaModificare, token);
            Titolo = $"Listini per il settore: {settore?.NomeSettore ?? "Sconosciuto"}";
            var data = await Q.GetListini(_idDaModificare, token);

            DataSource = data.Select(dto => new TariffaMap(dto)).ToList();
            await SetFocus(EscFocus);
        }

        protected async override Task OnSaving()
        {
            if (DataSource == null) return;
            _isClosing = true;

            var dtoSource = DataSource.Select(p => p.ToDto()).ToList();

            try
            {
                InfoLabel = "Salvataggio listini...";

                if (!await Q.SaveListini(_idDaModificare, dtoSource, token))
                {
                    InfoLabel = "Errore Database: modifica listini fallita";
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

    public partial class ListinoViewModel
    {
        #region DataSource

        private IList<TariffaMap> _datasource = [];
        public IList<TariffaMap> DataSource
        {
            get => _datasource;
            set => this.RaiseAndSetIfChanged(ref _datasource, value);
        }

        #endregion

        #region BindingT

        private TariffaMap _bindingT;
        public new TariffaMap BindingT
        {
            get => _bindingT;
            set => this.RaiseAndSetIfChanged(ref _bindingT, value);
        }

        #endregion


    }
}
