using Common.InterViewModels;
using DTO.Repository;
using System.Diagnostics;

namespace ViewModels
{
    public class PersonAddViewModel : PersonInputBase, IPersonAddViewModel
    {

        private IPersonRepository Q;

        public PersonAddViewModel(IPersonRepository Repository) : base()
        {
            Q = Repository ?? throw new ArgumentNullException(nameof(Repository));
            Titolo = "Aggiungi Nuovo Socio";
            FieldsVisibile = true;
            FieldsEnabled = true;
            BindingT = new();
        }

        protected override void OnFinalDestruction()
        {
            Q = null;
            DataSource = null;
        }

        protected override async Task OnLoading() => await SetFocus(CognomeFocus);
        

        protected async override Task OnSaving()
        {
            _isClosing = true;

            if (!await ValidaDati())
            {
                _isClosing = false; // Permette di riprovare dopo la validazione fallita
                return;
            }


            if (!int.TryParse(GetNumeroTessera, out int numeroTessera) || numeroTessera <= 0)
            {
                InfoLabel = "Numero Tessera non valido o uguale a zero";
                _isClosing = false;
                await SetFocus(TesseraFocus);
                return;
            }

            try
            {
                if (await Q.EsisteNumeroTessera(BindingT.NumeroTessera, token))
                {
                    InfoLabel = "Tessera già in uso";
                    _isClosing = false;
                    await SetFocus(TesseraFocus);
                    return;
                }

                if (int.TryParse(GetNumeroSocio, out int numeroSocio))
                {
                    // 2. Se la conversione riesce, controlliamo il valore
                    if (numeroSocio <= 0) { }
                    else
                    {
                        if (await Q.EsisteNumeroSocio(BindingT.NumeroSocio, token))
                        {
                            InfoLabel = "Codice Socio già in uso";
                            _isClosing = false;
                            await SetFocus(SocioFocus);
                            return;
                        }
                    }

                }
                else
                {
                    // 3. Se è stringa vuota o contiene lettere, finisce qui senza crash
                    // (In questo caso considerala come se fosse <= 0)
                    InfoLabel = "Codice Socio non può essere zero";
                    _isClosing = false;
                    await SetFocus(SocioFocus);
                    return;
                }

                if (await EsisteAnagrafica())
                {
                    InfoLabel = "Socio già registrato";
                    _isClosing = false;
                    await SetFocus(SocioFocus);
                    return;
                }

                InfoLabel = "Salvataggio in corso...";

                int newPersonId = await Q.AddPerson(BindingT.ToDto(), token);

                if (newPersonId == -1)
                {
                    InfoLabel = "Errore Db inserimento Socio";
                    _isClosing = false;
                    await SetFocus(CognomeFocus);
                    return;
                }

                await OnBack(newPersonId);
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
                await SetFocus(CognomeFocus);
            }
         
        }

        private async Task<bool> EsisteAnagrafica()
        {
            if (BindingT is null) return false;

            string srvcognome = (BindingT?.Cognome ?? "").PadRight(3);
            string srvnome = (BindingT?.Nome ?? "").PadRight(3);


            BindingT.CodiceUnivoco = string.Concat(
                                                srvcognome[..3],
                                                srvnome[..3],
                                                BindingT.Natoil.ToString());

            try
            {
                return await Q.EsisteCodiceUnivoco(BindingT.CodiceUnivoco, token);
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine("Operazione annullata dall'utente");
            }
            catch (Exception ex)
            {
                { Debug.WriteLine($"Esiste Anagrafica Error: {ex.Message}"); }
            }

            return false;



        }
    }
}
