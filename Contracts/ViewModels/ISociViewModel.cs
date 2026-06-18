using ReactiveUI;
using System.Reactive;

namespace Contracts.ViewModels
{
    public interface ISociViewModel : IRoutableViewModel
    {
        IObservable<Unit> SociToMenu { get; }
    }
}
