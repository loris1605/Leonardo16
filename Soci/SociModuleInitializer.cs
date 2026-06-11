using Common.InterViewModels;
using DTO.Repository;
using Models.Context;
using ReactiveUI;
using Splat;
using ViewModels;
using Views;

namespace Soci
{
    public static class SociModuleInitializer
    {
        public static void Initialize()
        {
            // 1. REGISTRAZIONE COMPONENTI DATI (Interni alla DLL)
            // Registriamo il DbContext specifico del modulo
            Locator.CurrentMutable.Register(() => new PeopleDbContext(), typeof(IPeopleDbContext));

            // CORRETTO: Spostiamo il GetService dentro l'ambito della Lambda () => ...
            // In questo modo, il DbContext verrà cercato solo quando verrà creato il Repository
            Locator.CurrentMutable.Register(() =>
            {
                var context = Locator.Current.GetService<IPeopleDbContext>();
                return new PersonRepository(context);
            }, typeof(IPersonRepository));

            

            // 2. REGISTRAZIONE COMPONENTI UI (Modello B - Usa e Getta)
            // CORRETTO: Spostiamo i resolver dentro la Lambda. 
            // Il ViewModel nascerà solo quando il costruttore verrà invocato dal thread UI di MainWindow
            Locator.CurrentMutable.Register(() =>
            {
                var screen = Locator.Current.GetService<IScreen>();
                //var repository = Locator.Current.GetService<IPersonRepository>();
                return new SociViewModel(screen);
            }, typeof(ISociViewModel));
            

            Locator.CurrentMutable.Register(() => new PersonGroupViewModel(Locator.Current.GetService<IPersonRepository>())
                                                                            , typeof(IPersonGroupViewModel));

            Locator.CurrentMutable.Register(() => new PersonAddViewModel(Locator.Current.GetService<IPersonRepository>())
                                                                            , typeof(IPersonAddViewModel));
            Locator.CurrentMutable.Register(() => new PersonDelViewModel(Locator.Current.GetService<IPersonRepository>())
                                                                            , typeof(IPersonDelViewModel));
            Locator.CurrentMutable.Register(() => new PersonUpdViewModel(Locator.Current.GetService<IPersonRepository>())
                                                                            , typeof(IPersonUpdViewModel));
            Locator.CurrentMutable.Register(() => new PersonSearchViewModel(Locator.Current.GetService<IPersonRepository>())
                                                                            , typeof(IPersonSearchViewModel));

            Locator.CurrentMutable.Register(() => new CodiceSocioAddViewModel(Locator.Current.GetService<IPersonRepository>())
                                                                            , typeof(ICodiceSocioAddViewModel));
            Locator.CurrentMutable.Register(() => new CodiceSocioDelViewModel(Locator.Current.GetService<IPersonRepository>())
                                                                            , typeof(ICodiceSocioDelViewModel));
            Locator.CurrentMutable.Register(() => new CodiceSocioUpdViewModel(Locator.Current.GetService<IPersonRepository>())
                                                                            , typeof(ICodiceSocioUpdViewModel));

            Locator.CurrentMutable.Register(() => new TesseraAddViewModel(Locator.Current.GetService<IPersonRepository>())
                                                                            , typeof(ITesseraAddViewModel));
            Locator.CurrentMutable.Register(() => new TesseraDelViewModel(Locator.Current.GetService<IPersonRepository>())
                                                                            , typeof(ITesseraDelViewModel));
            Locator.CurrentMutable.Register(() => new TesseraUpdViewModel(Locator.Current.GetService<IPersonRepository>())
                                                                            , typeof(ITesseraUpdViewModel));

            // Registriamo la View associata all'interfaccia e alla classe concreta per il Router
            Locator.CurrentMutable.Register(() => new SociView(), typeof(IViewFor<SociViewModel>));
            

            Locator.CurrentMutable.Register(() => new PersonGroupView(), typeof(IViewFor<PersonGroupViewModel>));

            Locator.CurrentMutable.Register(() => new PersonInputView(), typeof(IViewFor<PersonAddViewModel>));
            Locator.CurrentMutable.Register(() => new PersonInputView(), typeof(IViewFor<PersonDelViewModel>));
            Locator.CurrentMutable.Register(() => new PersonInputView(), typeof(IViewFor<PersonUpdViewModel>));
            Locator.CurrentMutable.Register(() => new PersonSearchView(), typeof(IViewFor<PersonSearchViewModel>));

            Locator.CurrentMutable.Register(() => new SocioInputView(), typeof(IViewFor<CodiceSocioAddViewModel>));
            Locator.CurrentMutable.Register(() => new SocioInputView(), typeof(IViewFor<CodiceSocioDelViewModel>));
            Locator.CurrentMutable.Register(() => new SocioInputView(), typeof(IViewFor<CodiceSocioUpdViewModel>));

            Locator.CurrentMutable.Register(() => new TesseraInputView(), typeof(IViewFor<TesseraAddViewModel>));
            Locator.CurrentMutable.Register(() => new TesseraInputView(), typeof(IViewFor<TesseraDelViewModel>));
            Locator.CurrentMutable.Register(() => new TesseraInputView(), typeof(IViewFor<TesseraUpdViewModel>));

            System.Diagnostics.Debug.WriteLine("***** [DLL-INIT] Soci Registrazioni Splat completate in modalità Lazy *****");
        }
    }
}
