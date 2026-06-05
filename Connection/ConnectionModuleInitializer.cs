using Common.InterViewModels;
using ReactiveUI;
using Splat;
using ViewModels;
using Views;

namespace Connection
{
    public static class ConnectionModuleInitializer
    {
        public static void Initialize()
        {
            Locator.CurrentMutable.Register(() =>
            {
                var screen = Locator.Current.GetService<IScreen>();
                return new ConnectionViewModel(screen);
            }, typeof(IConnectionViewModel));

            Locator.CurrentMutable.Register(() => new ConnectionView(), typeof(IViewFor<IConnectionViewModel>));
            Locator.CurrentMutable.Register(() => new ConnectionView(), typeof(IViewFor<ConnectionViewModel>));
        }

        
    }
}
