using Common.InterViewModels;
using DTO.Repository;

namespace ViewModels
{
    public class TariffaUpdViewModel : TariffaInputBase, ITariffaUpdViewModel
    {
        private ITariffaRepository Q;
       
        public TariffaUpdViewModel(ITariffaRepository Repository) : base()
        {
            Titolo = "Modifica Tariffa";
            FieldsVisibile = true; // Impostato come richiesto
            FieldsEnabled = true;

            Q = Repository ?? throw new ArgumentNullException(nameof(Repository));
        }

        protected override void OnFinalDestruction() => Q = null;

        protected override async Task OnLoading()
        {
            try
            {
                var data = await Q.FirstTariffa(_idDaModificare);

                if (data == null)
                {
                    InfoLabel = "Errore: Tariffa non trovata.";
                    FieldsEnabled = false;
                    return;
                }

                BindingT = new BindableObjects.TariffaMap(data);

                // In modifica, portiamo il focus sul nome all'avvio
                await SetFocus(NomeFocus);
            }
            catch (Exception ex)
            {
                InfoLabel = $"Errore caricamento: {ex.Message}";
                FieldsEnabled = false;
            }
        }

        protected override async Task OnSaving()
        {
            InfoLabel = "";
            _isClosing = true;
            // 1. Validazione (attesa perché async nella base)
            if (!await ValidaDati())
            {
                _isClosing = false;
                return;
            }

            try
            {
                // 2. Disabilita UI durante l'operazione
                FieldsEnabled = false;
                

                var input = BindingT.ToDto();

                // 3. Controllo duplicati (escludendo se stessa)
                if (await Q.EsisteNomeUpd(input))
                {
                    _isClosing = false;
                    InfoLabel = "Tariffa già registrata";
                    FieldsEnabled = true;
                    await SetFocus(NomeFocus);
                    return;
                }

                InfoLabel = "Aggiornamento in corso...";

                // 4. Aggiornamento Database
                if (!await Q.Upd(input))
                {
                    _isClosing = false;
                    InfoLabel = "Errore Db modifica Tariffa";
                    FieldsEnabled = true;
                    await SetFocus(NomeFocus);
                    return;
                }

                // 5. Successo
                await OnBack(_idDaModificare);
            }
            catch (Exception ex)
            {
                _isClosing = false;
                InfoLabel = $"Errore: {ex.Message}";
                FieldsEnabled = true;
                await SetFocus(NomeFocus);
            }
        }
    }
}
