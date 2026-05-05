using Common.InterViewModels;
using DTO.Repository;
using System.Diagnostics;

namespace ViewModels
{
    public class PersonDelViewModel : PersonInputBase, IPersonDelViewModel
    {
        private IPersonRepository Q;
                
        public PersonDelViewModel(IPersonRepository Repository) : base()
        {
            Q = Repository ?? throw new ArgumentNullException(nameof(Repository));
            Titolo = "Elimina Socio";
            FieldsEnabled = false;
            FieldsVisibile = false; ;
                       
        }

        protected override void OnFinalDestruction()
        {
            Q = null;
            DataSource = null;
        }

        protected override async Task OnLoading()
        {
            var data = await Q.FirstPerson(_idDaModificare, token);

            if (data == null || data.Id == 0)
            {
                InfoLabel = "Errore: Socio non trovato.";
                return; // Esci subito
            }

            BindingT = new BindableObjects.PersonMap(data);

            Titolo = $"Cancella Socio: {BindingT.Cognome}";

            await SetFocus(EscFocus,0);
        }

        protected async override Task OnSaving()
        {
            _isClosing = true;

            if (BindingT == null || BindingT.Id == 0)
            {
                _isClosing = false;
                InfoLabel = "Errore: Socio non valido.";
                await SetFocus(EscFocus);
                return;
            }

            InfoLabel = "Cancellazione in corso...";

            try
            {
                if (!await Q.Del(BindingT.ToDto(), token))
                {
                    InfoLabel = "Errore Db eliminazione person";
                    await SetFocus(EscFocus, 0);
                    return;
                }
                await OnBack(-100);
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
                await SetFocus(EscFocus);
            }
          
            
        }

        
    }
}
