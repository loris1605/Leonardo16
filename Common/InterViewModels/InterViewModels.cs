using ReactiveUI;
using System.Reactive;

namespace Common.InterViewModels
{
   
    public interface IMainWindowViewModel : IRoutableViewModel { }
    public interface ILoginViewModel : IRoutableViewModel
    {
        IObservable<Unit> LoginSuccesso { get; }
    }

    public interface IConnectionViewModel : IRoutableViewModel { }
    public interface IMenuViewModel : IRoutableViewModel
    {
        IObservable<Unit> MenuToLogin { get; }
        IObservable<Unit> MenuToSoci { get; }
    }

    public interface ISociViewModel : IRoutableViewModel
    {
        IObservable<Unit> SociToMenu { get; }
    }
    

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
    }

    public interface IPersonGroupViewModel : IRoutableViewModel
    {
        void SetHost(ISociScreen host);
        IObservable<Unit> GroupToPersonAdd { get; }
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

    public interface IConfigurazioneViewModel : IRoutableViewModel
    {
        void SetHost(IScreen host);
    }

    public interface IConfigurazioneScreen : IScreen
    {
        RoutingState GroupRouter { get; }
        RoutingState InputRouter { get; }
        bool GroupEnabled { get; set; }

        void AggiornaGridByInt(int id);
    }

    public interface iConfigurazioneCrudViewModel : IRoutableViewModel
    {
        void SetHost(IConfigurazioneScreen host);
        void SetIdDaModificare(int id);
    }

    public interface IOperatoreGroupViewModel : IRoutableViewModel
    {
        void SetHost(IConfigurazioneScreen host);
    }

    public interface IOperatoreAddViewModel : IRoutableViewModel, iConfigurazioneCrudViewModel { }
    public interface IOperatoreDelViewModel : IRoutableViewModel, iConfigurazioneCrudViewModel { }
    public interface IOperatoreUpdViewModel : IRoutableViewModel, iConfigurazioneCrudViewModel { }
    public interface IPermessoViewModel : IRoutableViewModel, iConfigurazioneCrudViewModel { }

    public interface IPostazioneGroupViewModel : IRoutableViewModel
    {
        void SetHost(IConfigurazioneScreen host);
    }
    public interface IPostazioneAddViewModel : IRoutableViewModel, iConfigurazioneCrudViewModel { }
    public interface IPostazioneDelViewModel : IRoutableViewModel, iConfigurazioneCrudViewModel { }
    public interface IPostazioneUpdViewModel : IRoutableViewModel, iConfigurazioneCrudViewModel { }
    public interface IRepartoViewModel : IRoutableViewModel, iConfigurazioneCrudViewModel { }

    public interface ISettoreGroupViewModel : IRoutableViewModel
    {
        void SetHost(IConfigurazioneScreen host);
    }
    public interface ISettoreAddViewModel : IRoutableViewModel, iConfigurazioneCrudViewModel { }
    public interface ISettoreDelViewModel : IRoutableViewModel, iConfigurazioneCrudViewModel { }
    public interface ISettoreUpdViewModel : IRoutableViewModel, iConfigurazioneCrudViewModel { }
    public interface IListinoViewModel : IRoutableViewModel, iConfigurazioneCrudViewModel { }

    public interface ITariffaGroupViewModel : IRoutableViewModel
    {
        void SetHost(IConfigurazioneScreen host);
    }
    public interface ITariffaAddViewModel : IRoutableViewModel, iConfigurazioneCrudViewModel { }
    public interface ITariffaDelViewModel : IRoutableViewModel, iConfigurazioneCrudViewModel { }
    public interface ITariffaUpdViewModel : IRoutableViewModel, iConfigurazioneCrudViewModel { }

    public interface ICassaScreen : IScreen
    {
        RoutingState CassaRouter { get; }
        RoutingState SettingsRouter { get; }
        Task OnClosing();

    }
    public interface ICassaViewModel : IRoutableViewModel
    {
        void SetHost(IScreen host);
        void SetPostazioneId(int id);   
    }

    public interface ICassaPostazioneViewModel : IRoutableViewModel
    {
        void SetHost(ICassaScreen host);
        void SetPostazioneId(int id);
        void SetPosizione(string numPosizione);
    }

    public interface IEntraSocioViewModel : IRoutableViewModel
    {
        void SetHost(ICassaScreen host);
        void SetPostazioneId(int id);
        void SetPosizione(string numPosizione);
    }

}


