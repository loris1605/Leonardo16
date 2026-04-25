using Models.Repository;

namespace Models
{
    public class LoginRepository : LoginROld, IDisposable, ILoginR { }
    public class MenuRepository : MenuR, IDisposable, IMenuR { }
    public class PersonRepository : PersonR, IDisposable, IPersonR { }
    public class PersonSearchRepository : PersonSearchR, IDisposable, IPersonSearchR { }
    public class SocioRepository : SocioR, IDisposable, ISocioR { }
    public class TesseraRepository : TesseraR, IDisposable, ITesseraR { }
    public class OperatoreRepository : OperatoreR, IDisposable, IOperatoreR { }

}
