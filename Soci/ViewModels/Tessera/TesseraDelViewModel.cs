using Common.InterViewModels;
using DTO.Repository;
using System.Diagnostics;
using ViewModels.BindableObjects;

namespace ViewModels
{
    public class TesseraDelViewModel : TesseraInputBase, ITesseraDelViewModel
    {
        private IPersonRepository Q;
        public TesseraDelViewModel(IPersonRepository Repository) : base()
        {
           FieldVisibile = false;
           FieldsEnabled = false;

           Q = Repository ?? throw new ArgumentNullException(nameof(Repository));

        }

        protected override void OnFinalDestruction()
        {
            Q = null;
        }
        
        protected override async Task OnLoading()
        {
            var data = await Q.FirstTessera(_idDaModificare, token);

            if (data == null)
            {
                InfoLabel = "Errore: Tesera non trovata nel database.";
                FieldsEnabled = false;
            }
            else
            {
                BindingT = new PersonMap(data);
                Titolo = "Elimina Tessera : " + GetNumeroTessera;
                Titolo1 = "per " + GetNomeCognome;
            }

            await SetFocus(EscFocus);

        }

        protected async override Task OnSaving()
        {
            _isClosing = true;

            try
            {
                InfoLabel = "Salvataggio in corso...";

                if (!await Q.DelTessera(BindingT.ToDto(), token))
                {
                    _isClosing = false;
                    InfoLabel = "Errore Db eliminazione person";
                    await SetFocus(EscFocus);
                    return;
                }

                await OnBack(_idRitorno);
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
