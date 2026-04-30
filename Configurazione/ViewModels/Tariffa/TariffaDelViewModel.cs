using Common.InterViewModels;
using DTO.Repository;
using Models.Repository;
using ReactiveUI;
using SysNet;

namespace ViewModels
{
    public class TariffaDelViewModel : TariffaInputBase, ITariffaDelViewModel
    {
        private ITariffaRepository Q;

        private readonly int _idDaModificare;

        public TariffaDelViewModel(IScreen host, int idtariffa, ITariffaRepository Repository) : base(host)
        {
            _idDaModificare = idtariffa;
            Titolo = "Cancella Settore";
            Q = Repository ?? throw new ArgumentNullException(nameof(Repository));
            FieldsEnabled = false;
        }

        protected override void OnFinalDestruction() => Q = null;

        protected override async Task OnLoading()
        {
            var data = await Q.FirstTariffa(_idDaModificare);

            BindingT = new BindableObjects.TariffaMap(data);

            if (GetCodiceTariffa == 0)
            {
                InfoLabel = "Errore: Tariffa non trovata nel database.";
                FieldsEnabled = false;
            }
            SetFocus(EscFocus);
        }

        protected async override Task OnSaving()
        {
            if (!await Q.Del(BindingT.ToDTO()))
            {
                InfoLabel = "Errore Db eliminazione Tariffa";
                SetFocus(EscFocus);
                return;
            }
            OnBack(-100);
        }
    }
}
