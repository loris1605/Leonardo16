using ReactiveUI;
using System.Reactive;

namespace Soci.Interfaces.ViewModels
{
    public interface ISociScreen : IScreen
    {
        RoutingState GroupRouter { get; }
        RoutingState InputRouter { get; }
        bool GroupEnabled { get; set; }

        void AggiornaGridByInt(int id);
        void AggiornaGridByObject(object model);
    }

    public interface iSociCrudViewModel : IRoutableViewModel
    {
        void SetHost(ISociScreen host);
        void SetIdDaModificare(int id);
        void SetIdRitorno(int id);
        IObservable<Unit> InputEsc { get; }
        IObservable<int> InputBack { get; }

    }

    public interface IPersonAddViewModel : IRoutableViewModel, iSociCrudViewModel { }
    public interface IPersonUpdViewModel : IRoutableViewModel, iSociCrudViewModel { }
    public interface IPersonDelViewModel : IRoutableViewModel, iSociCrudViewModel { }

    public interface IPersonSearchViewModel : IRoutableViewModel, iSociCrudViewModel { }
    public interface ICodiceSocioAddViewModel : IRoutableViewModel, iSociCrudViewModel { }
    public interface ICodiceSocioDelViewModel : IRoutableViewModel, iSociCrudViewModel { }
    public interface ICodiceSocioUpdViewModel : IRoutableViewModel, iSociCrudViewModel { }
    public interface ITesseraAddViewModel : IRoutableViewModel, iSociCrudViewModel { }
    public interface ITesseraDelViewModel : IRoutableViewModel, iSociCrudViewModel { }
    public interface ITesseraUpdViewModel : IRoutableViewModel, iSociCrudViewModel { }
}
