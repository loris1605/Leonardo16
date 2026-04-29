using Common.InterViewModels;
using DTO.Repository;
using ViewModels.BindableObjects;

namespace ViewModels
{
    public class PostazioneUpdViewModel : PostazioneInputBase, IPostazioneUpdViewModel
    {
        private IPostazioneRepository Q;

        public PostazioneUpdViewModel(IPostazioneRepository Repository) : base()
        {
            Titolo = "Modifica Postazione";
            FieldsEnabled = true;
            FieldsVisibile = true;
            Q = Repository ?? throw new ArgumentNullException(nameof(Repository));
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

            if (BindingT == null || BindingT.Id == 0)
            {
                InfoLabel = "Errore: Postazione non trovata.";
                FieldsEnabled = false;
                await SetFocus(EscFocus);
                return;
            }

            // 3. Focus iniziale sul nome
            await SetFocus(NomeFocus);
        }

        protected override async Task OnSaving()
        {
            if (!await ValidaDati()) return;

            try
            {
                // 2. Controllo Duplicati (escludendo se stesso tramite Dto/Id)
                if (await Q.EsisteNomeUpd(BindingT.ToDto(), token))
                {
                    InfoLabel = "Nome postazione già in uso da un'altra postazione";
                    await SetFocus(NomeFocus);
                    return;
                }

                InfoLabel = "Aggiornamento in corso...";

                // 3. Esecuzione Update
                if (!await Q.Upd(BindingT.ToDto(), token))
                {
                    InfoLabel = "Errore Database durante la modifica";
                    await SetFocus(NomeFocus);
                    return;
                }

                // 4. Successo: Ritorno alla grid con l'ID modificato
                await OnBack(_idDaModificare);
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                InfoLabel = $"Errore: {ex.Message}";
                await SetFocus(NomeFocus);
            }

        }
    }
}
