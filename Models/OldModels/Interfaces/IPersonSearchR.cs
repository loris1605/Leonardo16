using Models.Entity;

namespace Models.Repository
{
    public interface IPersonSearchR : IDisposable
    {
        int FirstIdByNumeroSocio(string numerosocio);
        int FirstIdByNumeroTessera(int numerotessera);
        List<PersonMap> LoadByCognome(string cognome);
        List<PersonMap> LoadByNato(int natoil);
        List<PersonMap> LoadByNome(string nome);
        List<PersonMap> LoadContainsCognome(string cognome);
        List<PersonMap> LoadContainsNome(string nome);
        List<PersonMap> LoadMaiorNato(int natoil);
        List<PersonMap> LoadMinorNato(int natoil);
        List<PersonMap> LoadStartByCognome(string cognome);
        List<PersonMap> LoadStartByNome(string nome);
    }
}