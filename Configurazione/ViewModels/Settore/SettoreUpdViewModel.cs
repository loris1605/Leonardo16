using Common.InterViewModels;
using DTO.Repository;
using ViewModels.BindableObjects;

namespace ViewModels
{
    public class SettoreUpdViewModel : SettoreInputBase, ISettoreUpdViewModel
    {
        private ISettoreRepository Q;
        
        public SettoreUpdViewModel(ISettoreRepository repository) : base()
        {
            Titolo = "Modifica Settore";
            FieldsEnabled = true;
            FieldsVisibile = true;
            Q = repository;
        }

        protected override void OnFinalDestruction() => Q = null;

        protected override async Task OnLoading()
        {

            await CaricaCombos();

            var data = await Q.FirstSettore(_idDaModificare);
            BindingT = new BindableObjects.SettoreMap(data);

            if (BindingT == null || BindingT.Id == 0)
            {
                InfoLabel = "Errore: Settore non trovato.";
                FieldsEnabled = false;
                await SetFocus(EscFocus);
                return;
            }

            await SetFocus(EscFocus);
        }

        private async Task CaricaCombos()
        {
            var data = await Q.LoadTipiSettore();
            TipoSettDataSource = data.Select(dto => new TipoSettoreMap(dto)).ToList();
            
        }

        protected override async Task OnSaving()
        {
            InfoLabel = "";
            if (!await ValidaDati()) return;

            try
            {
                // 2. Controllo duplicati (escludendo se stesso)
                if (await Q.EsisteNomeUpd(BindingT.ToDto(), token))
                {
                    InfoLabel = "Nome settore già in uso";
                    await SetFocus(NomeFocus);
                    return;
                }

                InfoLabel = "Aggiornamento in corso...";

                // 3. Esecuzione Update
                if (await Q.Upd(BindingT.ToDto(), token))
                {
                    // Successo: torniamo indietro
                    await OnBack(_idDaModificare);
                }
                else
                {
                    InfoLabel = "Errore Database durante la modifica";
                    await SetFocus(NomeFocus);
                }
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
