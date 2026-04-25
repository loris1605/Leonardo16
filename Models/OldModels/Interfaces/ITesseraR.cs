using Models.Entity;

namespace Models.Repository
{
    public interface ITesseraR
    {
        int Add(PersonMap map);
        bool Del(int id);
        void Dispose();
        bool EsisteNumeroTessera(int numerotessera);
        bool EsisteNumeroTesseraUpd(PersonMap dT);
        PersonMap SocioFirst(int codicesocio);
        PersonMap TesseraFirst(int codicetessera);
        bool Upd(PersonMap map);
    }
}