using Common.InterViewModels;
using DTO.Entity;
using DTO.Repository;
using ReactiveUI;
using System.Diagnostics;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using ViewModels.BindableObjects;

namespace ViewModels
{
    public partial class PersonSearchViewModel : PersonInputBase, IPersonSearchViewModel
    {
        private IPersonRepository Q;

        public PersonSearchViewModel(IPersonRepository Repository) : base()
        {
            Titolo = "Trova Socio";
            Q = Repository ?? throw new ArgumentNullException(nameof(Repository));

            BindingT = new();

            this.WhenActivated(d =>
            {
                this.WhenAnyValue(x => x.AnagraficaChecked)
                    .Where(value => value == true) // Filtra: procedi solo se è true
                    .ObserveOn(RxSchedulers.MainThreadScheduler) // Assicurati di essere sul thread UI
                    .Subscribe(async _ => await SetFocus(CognomeFocus))
                    .DisposeWith(d);

                this.WhenAnyValue(x => x.SocioChecked)
                    .Where(value => value == true) // Filtra: procedi solo se è true
                    .ObserveOn(RxSchedulers.MainThreadScheduler) // Assicurati di essere sul thread UI
                    .Subscribe(async _ => await SetFocus(SocioFocus))
                    .DisposeWith(d);

                this.WhenAnyValue(x => x.TesseraChecked)
                    .Where(value => value == true) // Filtra: procedi solo se è true
                    .ObserveOn(RxSchedulers.MainThreadScheduler) // Assicurati di essere sul thread UI
                    .Subscribe(async _ => await SetFocus(TesseraFocus))
                    .DisposeWith(d);
             

            });
        }

        protected override void OnFinalDestruction()
        {
            Q = null;
            DataSource = null;
        }

        protected override async Task OnLoading()
        {
            ResetAllCombos();
            AnagraficaChecked = true;
            await SetFocus(CognomeFocus);
        }

        protected async override Task OnSaving()
        {
            _isClosing = true;

            InfoLabel = "Ricerca in corso ...";

            try
            {
                // se il numero di tessera è diverso da zero esegue la ricerca solo sulla tessera
                if (TesseraChecked && !string.IsNullOrWhiteSpace(GetNumeroTessera))
                {
                    int personid = await Q.FirstIdPersonByNumeroTessera(GetNumeroTessera, token);
                    if (personid != 0) await OnBack(personid);
                    else
                    {
                        _isClosing = false;
                        InfoLabel = "Nessuna persona trovata";
                        await SetFocus(TesseraFocus);
                    }
                    return;
                }

                // se il numero socio è diverso da zero esegue la ricerca solo sul socio
                if (SocioChecked && !string.IsNullOrWhiteSpace(GetNumeroSocio))
                {
                    int personid = await Q.FirstIdPersonByNumeroSocio(GetNumeroSocio, token);

                    if (personid != 0) await OnBack(personid);
                    else
                    {
                        _isClosing = false;
                        InfoLabel = "Nessuna persona trovata";
                        await SetFocus(SocioFocus);
                    }
                    return;
                }

                await StartSearch(CognomeSelectedValue, NomeSelectedValue, NatoilSelectedValue, token);


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

        private void ResetAllCombos()
        {
            CognomeSelectedValue = 0;
            NomeSelectedValue = 0;
            NatoilSelectedValue = 0;
        }

        private async Task StartSearch(int cognomeFlag, int nomeFlag, int natoilFlag, CancellationToken ct)
        {
            List<PersonDTO> dbData = new();

            if (cognomeFlag > 0)
            {


                if (cognomeFlag == 1) dbData = await Q.LoadByCognomeExact(GetCognome, ct);
                else if (cognomeFlag == 2) dbData = await Q.LoadStartByCognome(GetCognome, ct);
                else dbData = await Q.LoadContainsCognome(GetCognome, ct);

                DataSource = dbData.Select(dto => new PersonMap(dto)).ToList();

                if (DataSource == null || DataSource.Count == 0)
                {
                    InfoLabel = "Nessuna persona trovata";
                    await SetFocus(CognomeFocus);
                    return;
                }

                //dopo il nome
                EstractNome();

                if (DataSource == null || DataSource.Count == 0)
                {
                    InfoLabel = "Nessuna persona trovata";
                    await SetFocus(NomeFocus);
                    return;
                }

                //dopo la data di nascita
                EstractNatoil();

            }
            else if (nomeFlag > 0)
            {
                if (nomeFlag == 1) dbData = await Q.LoadByNomeExact(GetNome, ct);
                else if (nomeFlag == 2) dbData = await Q.LoadStartByNome(GetNome, ct);
                else dbData = await Q.LoadContainsNome(GetNome, ct);

                DataSource = dbData.Select(dto => new PersonMap(dto)).ToList();

                if (DataSource == null || DataSource.Count == 0)
                {
                    InfoLabel = "Nessuna persona trovata";
                    await SetFocus(NomeFocus);
                    return;
                }

                //dopo la data di nascita
                EstractNatoil();

            }

            else if (natoilFlag > 0)
            {
                if (natoilFlag == 1) dbData = await Q.LoadByNatoilExact(Natoil, ct);
                else if (natoilFlag == 2) dbData = await Q.LoadMinorNato(Natoil, ct);
                else dbData = await Q.LoadMaiorNato(Natoil, ct);

                DataSource = dbData.Select(dto => new PersonMap(dto)).ToList();

            }

            if (DataSource == null || DataSource.Count == 0)
            {
                InfoLabel = "Nessuna persona trovata";
                await SetFocus(CognomeFocus);
                return;
            }

            //1.Caricamento iniziale dei dati(scegli la query più restrittiva per performance)
            OnBackFiltered();


        }

        private void Estract(System.Func<PersonMap, bool> condition)
        {
            try { DataSource = [.. DataSource.Where(condition)]; } catch (Exception) { DataSource = null; }
        }

        private void EstractNome()
        {
            if (NomeSelectedValue == 1) // uguale a
            {
                Estract(x => x.Nome.Equals(GetNome, StringComparison.CurrentCultureIgnoreCase));
            }
            else if (NomeSelectedValue == 2) // inizia con
            {
                Estract(x => x.Nome.StartsWith(GetNome, StringComparison.CurrentCultureIgnoreCase));
            }
            else if (NomeSelectedValue == 3) // che contiene
            {
                Estract(x => x.Nome.Contains(GetNome, StringComparison.CurrentCultureIgnoreCase));
            }
        }

        private void EstractNatoil()
        {
            if (NatoilSelectedValue == 1) // uguale a
            {
                Estract(x => x.Natoil == Natoil);
            }
            else if (NatoilSelectedValue == 2) // prima di
            {
                Estract(x => x.Natoil <= Natoil);
            }
            else if (NatoilSelectedValue == 3) // dopo di
            {
                Estract(x => x.Natoil >= Natoil);
            }
        }

        private void OnBackFiltered()
        {
            if (_host is ISociScreen sociHost)
            {
                // Svuota completamente lo stack del router di input
                sociHost.InputRouter.NavigateBack.Execute();
                sociHost.InputRouter.NavigationStack.Clear();
                sociHost.AggiornaGridByObject(DataSource);
                sociHost.GroupEnabled = true;
            }
        }

    }

    public partial class PersonSearchViewModel
    {
        #region Enabled

        #region TesseraEnabled

        private bool _mytesseraenabled;
        public bool TesseraEnabled
        {
            get => _mytesseraenabled;
            set => this.RaiseAndSetIfChanged(ref _mytesseraenabled, value);
        }

        #endregion

        #region SocioEnabled

        private bool _mysocioenabled;
        public bool SocioEnabled
        {
            get => _mysocioenabled;
            set => this.RaiseAndSetIfChanged(ref _mysocioenabled, value);
        }

        #endregion

        #region NomeEnabled

        private bool _mynomeenabled;
        public bool NomeEnabled
        {
            get => _mynomeenabled;
            set => this.RaiseAndSetIfChanged(ref _mynomeenabled, value);
        }

        #endregion

        #region CognomeEnabled

        private bool _mycognomeenabled;
        public bool CognomeEnabled
        {
            get => _mycognomeenabled;
            set => this.RaiseAndSetIfChanged(ref _mycognomeenabled, value);
        }

        #endregion

        #region NatoilEnabled

        private bool _mynatoilenabled;
        public bool NatoilEnabled
        {
            get => _mynatoilenabled;
            set => this.RaiseAndSetIfChanged(ref _mynatoilenabled, value);
        }

        #endregion

        #region CognomeComboEnabled

        private bool _mycognomecomboenabled;
        public bool CognomeComboEnabled
        {
            get => _mycognomecomboenabled;
            set => this.RaiseAndSetIfChanged(ref _mycognomecomboenabled, value);
        }

        #endregion

        #region NomeComboEnabled

        private bool _mynomecombonabled;
        public bool NomeComboEnabled
        {
            get => _mynomecombonabled;
            set => this.RaiseAndSetIfChanged(ref _mynomecombonabled, value);
        }

        #endregion

        #region NatoilComboEnabled

        private bool _mynatoilcomboenabled;
        public bool NatoilComboEnabled
        {
            get => _mynatoilcomboenabled;
            set => this.RaiseAndSetIfChanged(ref _mynatoilcomboenabled, value);
        }

        #endregion

        #endregion

        #region Combo SelectedValue

        #region CognomeSelectedValue

        private int _mycognomeselectedvalue;
        public int CognomeSelectedValue
        {
            get => _mycognomeselectedvalue;
            set => this.RaiseAndSetIfChanged(ref _mycognomeselectedvalue, value);
        }

        #endregion

        #region NomeSelectedValue

        private int _mynomeselectedvalue;
        public int NomeSelectedValue
        {
            get => _mynomeselectedvalue;
            set => this.RaiseAndSetIfChanged(ref _mynomeselectedvalue, value);
        }

        #endregion

        #region NatoilSelectedValue

        private int _mynatoilselectedvalue;
        public int NatoilSelectedValue
        {
            get => _mynatoilselectedvalue;
            set => this.RaiseAndSetIfChanged(ref _mynatoilselectedvalue, value);
        }

        #endregion

        #endregion

        #region Checked

        #region AnagraficaChecked

        //private static void OnAnagraficaCheckedChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    PersonSearchVM me = (PersonSearchVM)d;
        //    me.CognomeFocus = (bool)e.NewValue;
        //}

        private bool _myanagraficachecked;
        public bool AnagraficaChecked
        {
            get => _myanagraficachecked;
            set => this.RaiseAndSetIfChanged(ref _myanagraficachecked, value);
        }

        #endregion

        #region TesseraChecked
        //private static void OnTesseraCheckedChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    PersonSearchVM me = (PersonSearchVM)d;
        //    me.TesseraEnabled = (bool)e.NewValue;
        //    me.NumeroTesseraFocus = (bool)e.NewValue;
        //}

        private bool _mytesserachecked;
        public bool TesseraChecked
        {
            get => _mytesserachecked;
            set => this.RaiseAndSetIfChanged(ref _mytesserachecked, value);
        }

        #endregion

        #region SocioChecked

        //private static void OnSocioCheckedChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    PersonSearchVM me = (PersonSearchVM)d;
        //    me.SocioEnabled = (bool)e.NewValue;
        //    me.NumeroSocioFocus = (bool)e.NewValue;
        //}

        private bool _mysociochecked;
        public bool SocioChecked
        {
            get => _mysociochecked;
            set => this.RaiseAndSetIfChanged(ref _mysociochecked, value);
        }

        #endregion

        #endregion
    }
}
