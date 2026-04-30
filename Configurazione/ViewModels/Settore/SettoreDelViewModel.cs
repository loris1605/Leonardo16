using Common.InterViewModels;
using DTO.Repository;
using ViewModels.BindableObjects;

namespace ViewModels
{
    public class SettoreDelViewModel : SettoreInputBase, ISettoreDelViewModel
    {
        private ISettoreRepository Q;
        
        public SettoreDelViewModel(ISettoreRepository Repository) : base()
        {
            Titolo = "Cancella Settore";
            Q = Repository ?? throw new ArgumentNullException(nameof(Repository));
            // Disabilitiamo l'input ma teniamo visibili i campi
            FieldsEnabled = false;
            FieldsVisibile = true;
        }

        protected override void OnFinalDestruction() => Q = null;

        protected override async Task OnLoading()
        {
            await CaricaCombos();

            var data = await Q.FirstSettore(_idDaModificare);
    
            if (data == null || data.Id == 0)
            {
                InfoLabel = "Errore: Settore non trovato.";
                FieldsEnabled = false;
                await SetFocus(EscFocus);
                return;
            }

            BindingT = new BindableObjects.SettoreMap(data);
            Titolo = $"Cancella Settore: {BindingT.NomeSettore}";

            await SetFocus(EscFocus);
        }

        private async Task CaricaCombos()
        {
            var data = await Q.LoadTipiSettore();
            TipoSettDataSource = data.Select(dto => new TipoSettoreMap(dto)).ToList();
            
        }

        protected async override Task OnSaving()
        {
            _isClosing = true;

            if (BindingT == null || BindingT.Id == 0)
            {
                _isClosing = false;
                InfoLabel = "Errore: Settore non valido.";
                await SetFocus(EscFocus);
                return;
            }

            InfoLabel = "Cancellazione in corso...";

            try
            {
                // Esecuzione eliminazione
                if (!await Q.Del(BindingT.ToDto(), token))
                {
                    _isClosing = false;
                    InfoLabel = "Errore Database: impossibile eliminare il settore";
                    await SetFocus(EscFocus);
                    return;
                }

                // Successo: ritorno alla grid con flag di refresh totale
                await OnBack(-100);
            }
            catch (Exception ex)
            {
                _isClosing = false;
                InfoLabel = $"Errore critico: {ex.Message}";
                await SetFocus(EscFocus);
            }
        }

    }
}
