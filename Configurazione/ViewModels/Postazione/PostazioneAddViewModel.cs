using Common.InterViewModels;
using DTO.Repository;
using ViewModels.BindableObjects;

namespace ViewModels
{
    public class PostazioneAddViewModel : PostazioneInputBase, IPostazioneAddViewModel
    {
        private IPostazioneRepository Q;

        public PostazioneAddViewModel(IPostazioneRepository Repository) : base()
        {
            Titolo = "Aggiungi Nuova Postazione";
            FieldsVisibile = true;
            FieldsEnabled = true;
            Q = Repository ?? throw new ArgumentNullException(nameof(Repository));
            BindingT = new PostazioneMap();
        }

        protected override void OnFinalDestruction() => Q = null;

        protected override async Task OnLoading()
        {
            await CaricaCombos();
            await SetFocus(NomeFocus);
            
        }

        private async Task CaricaCombos()
        {
            var dataTipoPostazione = await Q.LoadTipiPostazione();
            TipoPostDataSource = dataTipoPostazione.Select(dto => new TipoPostazioneMap(dto)).ToList();

            var dataTipoRientro = await Q.LoadTipiRientro();
            TipoRientroDataSource = dataTipoRientro.Select(dto => new TipoRientroMap(dto)).ToList();


            // Seleziona il primo elemento solo se la lista non è vuota
            if (TipoPostDataSource?.Count > 0)
                BindingT.CodiceTipoPostazione = TipoPostDataSource[0].Id;

            if (TipoRientroDataSource?.Count > 0)
                BindingT.CodiceTipoRientro = TipoRientroDataSource[0].Id;
        }

        protected async override Task OnSaving()
        {
            _isClosing = true;
            // 1. Validazione Dati (ora è un Task, serve await)
            if (!await ValidaDati())
            {

                _isClosing = false; // Permette di riprovare dopo la validazione fallita
                return;
            }

            
            try
            {
                // 2. Controllo Duplicati
                if (await Q.EsisteNome(BindingT.ToDto(), token))
                {
                    _isClosing = false;
                    InfoLabel = "Postazione già registrata";
                    await SetFocus(NomeFocus);
                    return;
                }

                InfoLabel = "Salvataggio in corso...";

                // 3. Inserimento
                int newPostazioneId = await Q.Add(BindingT.ToDto(), token);

                if (newPostazioneId == -1)
                {
                    _isClosing = false;
                    InfoLabel = "Errore Db inserimento Postazione";
                    await SetFocus(NomeFocus);
                    return;
                }

                // 4. Successo: Ritorno protetto
                await OnBack(newPostazioneId);
            }
            catch (OperationCanceledException) { _isClosing = false; }
            catch (Exception ex)
            {
                _isClosing = false; 
                InfoLabel = $"Errore: {ex.Message}";
                await SetFocus(NomeFocus);
            }

        }
    }
}
