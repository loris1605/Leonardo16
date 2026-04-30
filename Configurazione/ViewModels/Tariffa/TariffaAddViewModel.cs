using Common.InterViewModels;
using DTO.Repository;
using ReactiveUI;

namespace ViewModels
{
    public class TariffaAddViewModel : TariffaInputBase, ITariffaAddViewModel
    {
        private ITariffaRepository Q;

        public TariffaAddViewModel(ITariffaRepository Repository) : base()
        {
            Titolo = "Aggiungi Nuova Tariffa";
            FieldsVisibile = true;
            FieldsEnabled = true;
            Q = Repository ?? throw new ArgumentNullException(nameof(Repository));
        }

        protected override void OnFinalDestruction() => Q = null;

        protected override async Task OnLoading()
        {
            await SetFocus(NomeFocus);
            
        }

        protected async override Task OnSaving()
        {
            if (!await ValidaDati()) return;

            try
            {
                // 2. Controllo Duplicati
                if (await Q.EsisteNome(BindingT.ToDto(), token))
                {
                    InfoLabel = "Tariffa già registrata";
                    await SetFocus(NomeFocus);
                    return;
                }

                InfoLabel = "Salvataggio in corso...";

                // 3. Inserimento a Database
                int newTariffaId = await Q.Add(BindingT.ToDto(), token);

                if (newTariffaId == -1)
                {
                    InfoLabel = "Errore Database: inserimento fallito";
                    await SetFocus(NomeFocus);
                    return;
                }

                // 4. Successo: Ritorno protetto
                await OnBack(newTariffaId);
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
