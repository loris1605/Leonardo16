using Common.InterViewModels;
using DTO.Repository;
using System.Diagnostics;

namespace ViewModels
{
    public class OperatoreAddViewModel : OperatoreInputBase, IOperatoreAddViewModel
    {
        private IOperatoreRepository Q;
        
        public OperatoreAddViewModel(IOperatoreRepository Repository) : base()
        {
            
            Titolo = "Aggiungi Nuovo Operatore";
            FieldsVisibile = true;
            FieldsEnabled = true;
            Q = Repository ?? throw new ArgumentNullException(nameof(Repository));
            BindingT = new();
        }

        protected override void OnFinalDestruction() => Q = null;

        protected override async Task OnLoading() => await SetFocus(NomeFocus);


        protected async override Task OnSaving()
        {
            _isClosing = true;
            // 1. Validazione Dati (ora è un Task, serve await)
            if (!await ValidaDati())
            { 
          
                _isClosing = false; // Permette di riprovare dopo la validazione fallita
                return;
            }

            // 2. Controllo duplicati con CancellationToken (ereditato dalla base)
            try
            {
                if (await Q.EsisteNome(BindingT.ToDto(), token))
                {
                    _isClosing = false;
                    InfoLabel = "Operatore già registrato";
                    await SetFocus(NomeFocus);
                    return;
                }

                InfoLabel = "Salvataggio in corso...";

                // 3. Impostazioni pre-salvataggio
                BindingT.CodicePerson = -2; // Logica specifica per l'anagrafica operatore

                // 4. Inserimento a Database
                int newOperatoreId = await Q.Add(BindingT.ToDto(), token);

                if (newOperatoreId == -1)
                {
                    _isClosing = false;
                    InfoLabel = "Errore Db inserimento Operatore";
                    await SetFocus(NomeFocus);
                    return;
                }

                // 5. Ritorno alla grid (ora è un Task asincrono e protetto)
                await OnBack(newOperatoreId);
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine("Salvataggio annullato.");
                _isClosing = false;
            }
            catch (Exception ex)
            {
                _isClosing = false;
                InfoLabel = $"Errore: {ex.Message}";
                await SetFocus(NomeFocus);
            }
        }
    }
}
