using Models.Entity;
using Models.Interfaces;

namespace Models.Repository
{
    public interface IPersonR : IDisposable, IGroupQ<PersonMap>
    {
        int Add(PersonMap map);
        bool Del(int id);
        bool EsisteCodiceUnivoco(string codiceunivoco);
        bool EsisteCodiceUnivoco(string codiceunivoco, int idperson);
        PersonMap First(int id);
        bool HasCodiciSocio(int idperson);
        bool Upd(PersonMap map);
    }
}