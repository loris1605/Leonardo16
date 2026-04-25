using Models.Repository;
using SysNet;

namespace Models
{
    public class ServiceM
    {
        public ILoginR GetLoginRepository => Create<LoginRepository>.Instance();
        public IMenuR GetMenuRepository => Create<MenuRepository>.Instance();
        public IPersonR GetPersonRepository => Create<PersonRepository>.Instance();
        public IPersonSearchR GetPersonSearchRepository => Create<PersonSearchRepository>.Instance();
        public ISocioR GetSocioRepository => Create<SocioRepository>.Instance();
        public ITesseraR GetTesseraRepository => Create<TesseraRepository>.Instance();
        public IOperatoreR GetOperatoreRepository => Create<OperatoreRepository>.Instance();
    }
}
