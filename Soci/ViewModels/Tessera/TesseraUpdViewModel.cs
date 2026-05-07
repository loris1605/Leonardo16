using Common.InterViewModels;
using DTO.Repository;
using System.Diagnostics;
using ViewModels.BindableObjects;

namespace ViewModels
{
    public class TesseraUpdViewModel : TesseraInputBase, ITesseraUpdViewModel
    {
        private IPersonRepository Q;
                
        public TesseraUpdViewModel(IPersonRepository Repository = null) : base()
        {
            FieldVisibile = true;
            FieldsEnabled = true;
            FieldsVisibile = true;

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
                Titolo = "Modifica Tessera : " + GetNumeroTessera;
                Titolo1 = "per " + GetNomeCognome;
            }
    
            await SetFocus(NumeroTesseraFocus);
        }

        protected async override Task OnSaving()
        {

            if (BindingT is null) return;

            _isClosing = true;

            try
            {
                if (int.TryParse(GetNumeroTessera, out int numeroTessera))
                {
                    // 2. Se la conversione riesce, controlliamo il valore
                    if (numeroTessera <= 0) { }
                    else
                    {
                        if (await Q.EsisteNumeroTesseraUpd(BindingT.ToDto(), token))
                        {
                            _isClosing = false;
                            InfoLabel = "Tessera già in uso";
                            await SetFocus(NumeroTesseraFocus);
                            return;
                        }
                    }

                }
                else
                {
                    // 3. Se è stringa vuota o contiene lettere, finisce qui senza crash
                    // (In questo caso considerala come se fosse <= 0)
                    _isClosing = false;
                    InfoLabel = "Numero Tessera non può essere zero";
                    await SetFocus(NumeroTesseraFocus);
                    return;
                }

                InfoLabel = "Aggiornamento in corso...";

                if (!await Q.UpdTessera(BindingT.ToDto(), token))
                {
                    _isClosing = false;
                    InfoLabel = "Errore Db modifica person";
                    await OnNumeroTesseraFocus();
                    return;
                }

                await OnBack(_idRitorno);
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine("Salvataggio annullato.");
                _isClosing = false;
                await SetFocus(NumeroTesseraFocus);
                return;
            }
            catch (Exception ex)
            {
                _isClosing = false;
                InfoLabel = $"Errore: {ex.Message}";
                await SetFocus(NumeroTesseraFocus);
                return;
            }
  
            
        }

    }
}
