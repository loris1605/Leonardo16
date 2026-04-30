using DTO.Repository;
using Models.Repository;
using ReactiveUI;
using SysNet;
using System.Reactive.Concurrency;
using ViewModels.BindableObjects;

namespace ViewModels
{
    public partial class PermessiViewModel : OperatoreInputBase
    {
        private IPermessoRepository Q;
        
        public PermessiViewModel(IPermessoRepository Repository) : base()
        {
            Q = Repository ?? throw new ArgumentNullException(nameof(Repository));
        }

        protected override void OnFinalDestruction()
        {
            Q = null;
            base.OnFinalDestruction();
        }

        protected override async Task OnLoading()
        {
            // 1. Recupero dati operatore per il titolo
            var operatore = await Q.FirstOperatore(_idDaModificare, token);
            Titolo = $"Permessi per l'operatore: {operatore?.NomeOperatore ?? "Sconosciuto"}";

            // 2. Caricamento della lista dei permessi
            var data = await Q.GetPermessi(_idDaModificare, token);

            // Trasformiamo i DTO in Map per il binding con le CheckBox nella Grid
            DataSource = data.Select(dto => new PostazioneElencoMap(dto)).ToList();

            // 3. Focus asincrono
            await SetFocus(EscFocus);
        }

        protected async override Task OnSaving()
        {
            if (DataSource == null) return;

            // Trasformiamo tutta la lista modificata di nuovo in DTO
            var dtoSource = DataSource.Select(p => p.ToDto()).ToList();

            try
            {
                InfoLabel = "Salvataggio permessi...";

                if (!await Q.SavePermessi(_idDaModificare, dtoSource, token))
                {
                    InfoLabel = "Errore Database: modifica permessi fallita";
                    await SetFocus(EscFocus);
                    return;
                }

                // Successo: ritorno protetto
                await OnBack(_idDaModificare);
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                InfoLabel = $"Errore: {ex.Message}";
                await SetFocus(EscFocus);
            }
        }


    }

    public partial class PermessiViewModel
    {
        #region DataSource

        private IList<PostazioneElencoMap> _datasource = [];
        public IList<PostazioneElencoMap> DataSource
        {
            get => _datasource;
            set => this.RaiseAndSetIfChanged(ref _datasource, value);
        }

        #endregion

        #region BindingT

        private PostazioneElencoMap _bindingT;
        public new PostazioneElencoMap BindingT
        {
            get => _bindingT;
            set => this.RaiseAndSetIfChanged(ref _bindingT, value);
        }

        #endregion


    }
}
