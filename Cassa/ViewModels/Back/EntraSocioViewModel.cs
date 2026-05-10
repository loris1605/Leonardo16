using Common.InterViewModels;
using DTO.Repository;
using ReactiveUI;
using Splat;
using System.Reactive;
using System.Reactive.Linq;

namespace ViewModels
{
    public partial class EntraSocioViewModel : BaseViewModel, IEntraSocioViewModel
    {
        private int _postazioneId;
        private ICassaScreen _host;
        private string _posizione;

        private IStrisciataRepository _strisciataRepository;

        public EntraSocioViewModel(IStrisciataRepository strisciataRepository) : base()
        {
            _strisciataRepository = strisciataRepository ?? throw new ArgumentNullException(nameof(strisciataRepository));
        }

        protected override async Task OnLoading()
        {
            await _strisciataRepository.DevelopStrisciate(token);
            await SetFocus(TesseraFocus);
        }

        public void SetHost(ICassaScreen host) => _host = host;

        public void SetPostazioneId(int posizioneId)
        {
            _postazioneId = posizioneId;
        }

        public void SetPosizione(string posizione)
        {
            _posizione = posizione;
        }

        protected async override Task OnEsc()
        {
            var cassaPostazioneVm = Locator.Current.GetService<ICassaPostazioneViewModel>();
            if (cassaPostazioneVm is not null)
            {
                cassaPostazioneVm.SetHost(_host);
                cassaPostazioneVm.SetPostazioneId(_postazioneId);
                cassaPostazioneVm.SetPosizione(_posizione);

                await _host.CassaRouter.NavigateAndReset.Execute(cassaPostazioneVm);
            }
        }
    }

    public partial class EntraSocioViewModel
    {
        public Interaction<Unit, Unit> TesseraFocus { get; } = new();
    }
}
