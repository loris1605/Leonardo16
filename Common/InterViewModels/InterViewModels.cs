using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.InterViewModels
{
    public interface ILoginViewModel : IRoutableViewModel { }
    public interface IConnectionViewModel : IRoutableViewModel { }
    public interface IMenuViewModel : IRoutableViewModel { }
    public interface IConfigurazioneViewModel : IRoutableViewModel { }
    public interface IOperatoreGroupViewModel : IRoutableViewModel { }
    public interface IOperatoreAddViewModel : IRoutableViewModel { }
    public interface IOperatoreDelViewModel : IRoutableViewModel { }
    public interface IOperatoreUpdViewModel : IRoutableViewModel { }

}
