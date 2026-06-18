using ReactiveUI;
using System.Reactive;

namespace Contracts.ViewModels
{
    public interface ILoginViewModel : IRoutableViewModel
    {
        IObservable<Unit> LoginSuccesso { get; }
    }
}
