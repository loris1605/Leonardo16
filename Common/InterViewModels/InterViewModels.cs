using ReactiveUI;

namespace Common.InterViewModels
{
   
    public interface IMainWindowViewModel : IRoutableViewModel { }

    public interface ILoginViewModel : IRoutableViewModel
    {
        void SetHost(IScreen host);
    }

    public interface IConnectionViewModel : IRoutableViewModel
    {
        void SetHost(IScreen host);
    }

    public interface IMenuViewModel : IRoutableViewModel
    {
        void SetHost(IScreen host);
    }

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
}


