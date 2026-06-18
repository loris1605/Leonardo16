using Soci.Interfaces.Core;
using Soci.Interfaces.ViewModels;
using System.Diagnostics;
using ViewModels.BindableObjects;

namespace ViewModels
{
    public class CodiceSocioUpdViewModel : CodiceSocioInputBase, ICodiceSocioUpdViewModel
    {
        private IPersonRepository Q;
        

        public CodiceSocioUpdViewModel(IPersonRepository Repository) : base()
        {
            FieldsEnabled = true;
            FieldsVisibile = true;
            FieldVisibile = false;

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
                await SetFocus(EscFocus);
            }
            else
            {
                BindingT = new PersonMap(data);
                Titolo = "Modifica Codice Socio per ";
                Titolo1 = "per " + GetNomeCognome;
                await SetFocus(NumeroSocioFocus);
            }
            
        }

        protected override async Task OnSaving()
        {
            _isClosing = true;
            if (BindingT == null || BindingT.Id == 0)
            {
                _isClosing = false;
                InfoLabel = "Errore: Socio non valido.";
                await SetFocus(EscFocus);
                return;
            }

            InfoLabel = "Aggiornamento in corso...";

            try
            {
                if (int.TryParse(GetNumeroSocio, out int numeroSocio))
                {
                    // 2. Se la conversione riesce, controlliamo il valore
                    if (numeroSocio <= 0) { }
                    else
                    {
                        if (await Q.EsisteNumeroSocioUpd(BindingT.ToDto(), token))
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

                InfoLabel = "";

                if (!await Q.UpdSocio(BindingT.ToDto(), token))
                {
                    _isClosing = false;
                    InfoLabel = "Errore Db modifica person";
                    await SetFocus(NumeroSocioFocus);
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
