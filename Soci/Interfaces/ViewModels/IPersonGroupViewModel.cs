using Common.InterViewModels;
using ReactiveUI;
using System.Reactive;

namespace Soci.Interfaces.ViewModels
{
    public interface IPersonGroupViewModel : IRoutableViewModel
    {
        void SetHost(ISociScreen host);
        IObservable<Unit> GroupToPersonAdd { get; }
        IObservable<int> GroupToPersonDel { get; }
        IObservable<int> GroupToPersonUpd { get; }
        IObservable<int> GroupToCodiceSocioAdd { get; }
        IObservable<int> GroupToCodiceSocioDel { get; }
        IObservable<(int id, int idRitorno)> GroupToCodiceSocioUpd { get; }
        IObservable<Unit> GroupToPersonSearch { get; }
        IObservable<(int id, int idRitorno)> GroupToTesseraAdd { get; }
        IObservable<(int id, int idRitorno)> GroupToTesseraDel { get; }
        IObservable<(int id, int idRitorno)> GroupToTesseraUpd { get; }
    }
}
