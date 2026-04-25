using Models.Entity;

namespace Models.Repository
{
    public interface ISocioR : IDisposable
    {
        int Add(PersonMap map);
        bool Del(int id);
        bool EsisteNumeroSocio(string numerosocio);
        bool EsisteNumeroSocio(string numerosocio, int idsocio);
        PersonMap First(int id);
        bool HasCodiciTessera(int idsocio);
        PersonMap SocioFirst(int codicesocio);
        bool Upd(PersonMap map);
    }
}