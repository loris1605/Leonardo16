using Common.InterViewModels;
using DTO.Repository;
using System.Diagnostics;
using ViewModels.BindableObjects;

namespace ViewModels
{
    public class CodiceSocioDelViewModel : CodiceSocioInputBase, ICodiceSocioDelViewModel
    {
        private IPersonRepository Q;
        
        public CodiceSocioDelViewModel(IPersonRepository Repository = null) : base()
        {
           FieldsVisibile = false;
            FieldsEnabled = false;

            Q = Repository ?? throw new ArgumentNullException(nameof(Repository));
            
        }

        protected override void OnFinalDestruction()
        {
            Q = null;
        }

        protected override async Task OnLoading()
        {
            var data = await Q.FirstSocio(_idDaModificare, token);
            token.ThrowIfCancellationRequested();
            if (data == null)
            {
                InfoLabel = "Errore: Socio non trovato nel database.";
                FieldsEnabled = false;

            }
            else
            {
                BindingT = new PersonMap(data);
                Titolo = "Elimina Codice Socio : " + GetNumeroSocio;
                Titolo1 = "per " + GetNomeCognome;
            }
            
            await SetFocus(EscFocus);

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
                if (!await Q.DelSocio(BindingT.ToDto(), token))
                {
                    InfoLabel = "Errore Db eliminazione person";
                    await SetFocus(EscFocus);
                    return;
                }
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

            
            
            await OnBack(_idRitorno);
        }
    }
}
