using Common.InterViewModels;
using Splat;
using System.Reactive.Linq;

namespace ViewModels
{
    public class EntraSocioViewModel : BaseViewModel, IEntraSocioViewModel
    {
        private int _postazioneId;
        private ICassaScreen _host;
        private string _posizione;

        public EntraSocioViewModel() : base()
        {

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
}
