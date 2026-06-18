using Soci.Interfaces.Core;
using Soci.Interfaces.ViewModels;
using System.Diagnostics;
using ViewModels.BindableObjects;

namespace ViewModels
{
    public class TesseraAddViewModel : TesseraInputBase, ITesseraAddViewModel
    {
        private IPersonRepository Q;
        
        private int idtessera;

        public TesseraAddViewModel(IPersonRepository Repository) : base()
        {
            Titolo = "Nuova Tessera";
 
            FieldsVisibile = true;
            FieldsEnabled = true;
            Q = Repository ?? throw new ArgumentNullException(nameof(Repository));

        }

        protected override void OnFinalDestruction()
        {
            Q = null;
        }

        protected override async Task OnLoading()
        {

            var dto = await Q.FirstSocio(_idDaModificare, token);
            token.ThrowIfCancellationRequested();
            if (dto == null)
            {
                InfoLabel = "Errore: Socio non trovato nel database.";
                FieldsEnabled = false;
                await SetFocus(EscFocus);
            }
            else
            {
                BindingT = new PersonMap(dto);
                Titolo = "Nuova Tessera per " + GetNomeCognome;
                Titolo1 = "Numero Socio : " + GetNumeroSocio;
                BindingT.NumeroTessera = string.Empty;
                await OnNumeroTesseraFocus();
            }
            
        }

        protected async override Task OnSaving()
        {
            _isClosing = true;

            try
            {
                if (int.TryParse(GetNumeroTessera, out int numeroTessera))
                {
                    // 2. Se la conversione riesce, controlliamo il valore
                    if (numeroTessera <= 0) { _isClosing = false; }
                    else
                    {
                        if (await Q.EsisteNumeroTessera(BindingT.NumeroTessera, token))
                        {
                            _isClosing = false;
                            InfoLabel = "Tessera già in uso";
                            await OnNumeroTesseraFocus(); 
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

                InfoLabel = "Salvataggio in corso...";

                idtessera = await Q.AddTessera(BindingT.ToDto(), token);

                if (idtessera == -1)
                {
                    _isClosing = false;
                    InfoLabel = "Errore durante il salvataggio. Verificare i dati e riprovare.";
                    await OnNumeroTesseraFocus();
                }
                else
                {
                    await OnBack(_idRitorno);
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
                await OnNumeroTesseraFocus();
            }

            
            
        }
    }
}
