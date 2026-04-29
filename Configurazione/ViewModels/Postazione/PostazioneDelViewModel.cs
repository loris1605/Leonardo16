using Common.InterViewModels;
using DTO.Repository;
using ViewModels.BindableObjects;

namespace ViewModels
{
    public class PostazioneDelViewModel : PostazioneInputBase, IPostazioneDelViewModel
    {
        private IPostazioneRepository Q;
       
        public PostazioneDelViewModel(IPostazioneRepository Repository) : base()
        {
            
            Titolo = "Cancella Postazione";

            Q = Repository ?? throw new ArgumentNullException(nameof(Repository));

            // In cancellazione disabilitiamo l'input ma mostriamo i dati
            FieldsEnabled = false;
            FieldsVisibile = true;
        }

        protected override void OnFinalDestruction() => Q = null;

        protected override async Task OnLoading()
        {
            var dataTipoPostazione = await Q.LoadTipiPostazione();
            TipoPostDataSource = dataTipoPostazione.Select(dto => new TipoPostazioneMap(dto)).ToList();

            var dataTipoRientro = await Q.LoadTipiRientro();
            TipoRientroDataSource = dataTipoRientro.Select(dto => new TipoRientroMap(dto)).ToList();

            var data = await Q.FirstPostazione(_idDaModificare);
            BindingT = new BindableObjects.PostazioneMap(data);
            Titolo = $"Cancella Postazione: {BindingT.NomePostazione}";


            if (GetCodicePostazione == 0)
            {
                InfoLabel = "Errore: Postazione non trovata nel database.";
                FieldsEnabled = false;
                await SetFocus(EscFocus);
                return;
            }
            await SetFocus(EscFocus);
        }

        protected async override Task OnSaving()
        {
            if (BindingT == null || BindingT.Id == 0) return;

            try
            {
                // Esecuzione eliminazione
                if (!await Q.Del(BindingT.ToDto(), token))
                {
                    InfoLabel = "Errore Database: impossibile eliminare la postazione";
                    await SetFocus(EscFocus);
                    return;
                }

                InfoLabel = "Cancellazione in corso...";

                // Successo: refresh totale della grid (-100)
                await OnBack(-100);
            }
            catch (Exception ex)
            {
                InfoLabel = $"Errore critico: {ex.Message}";
                await SetFocus(EscFocus);
            }
        }
    }
}
