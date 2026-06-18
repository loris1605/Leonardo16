using Soci.Interfaces.Core;
using Soci.Interfaces.ViewModels;
using System.Diagnostics;
using ViewModels.BindableObjects;

namespace ViewModels
{
    public class CodiceSocioAddViewModel : CodiceSocioInputBase, ICodiceSocioAddViewModel
    {
        private IPersonRepository Q;
                
        public CodiceSocioAddViewModel(IPersonRepository Repository) : base()
        {
           
            Titolo = "Nuovo Codice Socio";
            
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
            var dto = await Q.FirstPerson(_idDaModificare, token);
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
                Titolo1 = "per " + GetNomeCognome;
                BindingT.NumeroSocio = string.Empty;
                BindingT.NumeroTessera = string.Empty;
                await SetFocus(NumeroSocioFocus);
            }
  
        }
       

        private int idsocio;

        protected async override Task OnSaving()
        {
            _isClosing = true;

            try
            {
                if (int.TryParse(GetNumeroSocio, out int numeroSocio))
                {
                    // 2. Se la conversione riesce, controlliamo il valore
                    if (numeroSocio <= 0) { }
                    else
                    {
                        if (await Q.EsisteNumeroSocio(BindingT.NumeroSocio, token))
                        {
                            _isClosing = false;
                            InfoLabel = "Codice Socio già in uso";
                            await SetFocus(NumeroSocioFocus);
                            return;
                        }
                    }
                }
                else
                {
                    // 3. Se è stringa vuota o contiene lettere, finisce qui senza crash
                    // (In questo caso considerala come se fosse <= 0)
                    _isClosing = false;
                    InfoLabel = "Codice Socio non può essere zero";
                    await SetFocus(NumeroSocioFocus);
                    return;
                }

                if (int.TryParse(GetNumeroTessera, out int numeroTessera))
                {
                    // 2. Se la conversione riesce, controlliamo il valore
                    if (numeroTessera <= 0) { }
                    else
                    {
                        if (await Q.EsisteNumeroTessera(BindingT.NumeroTessera, token))
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

                InfoLabel = "Salvataggio in corso...";

                idsocio = await Q.AddCodiceSocio(BindingT.ToDto(), token);

                if (idsocio == -1)
                {
                    _isClosing = false;
                    await SetFocus(NumeroSocioFocus);
                }
                else
                {
                    await OnBack(_idDaModificare);
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
                await SetFocus(NumeroSocioFocus);
            }

            
        }
    }
}
