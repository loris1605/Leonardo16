using Models.Repository;
using Models.Tables;
using Soci.DTO.Entity;

namespace Soci.Interfaces.Core
{
    public interface IPersonRepository : IBaseRepository<Person>
    {
        Task<int> AddCodiceSocio(PersonDTO map, CancellationToken ctk = default);
        Task<int> AddPerson(PersonDTO map, CancellationToken ctk = default);
        Task<int> AddTessera(PersonDTO map, CancellationToken ctk = default);
        Task<bool> DelSocio(PersonDTO map, CancellationToken ctk = default);
        Task<bool> DelTessera(PersonDTO map, CancellationToken ctk = default);
        Task<bool> EsisteCodiceUnivoco(string codiceunivoco, CancellationToken ctk = default);
        Task<bool> EsisteCodiceUnivoco(string codiceunivoco, int id, CancellationToken ctk = default);
        Task<bool> EsisteNumeroSocio(string numeroSocio, CancellationToken ctk = default);
        Task<bool> EsisteNumeroSocioUpd(PersonDTO dT, CancellationToken ctk = default);
        Task<bool> EsisteNumeroTessera(string numeroTessera, CancellationToken ctk = default);
        Task<bool> EsisteNumeroTesseraUpd(PersonDTO dT, CancellationToken ctk = default);
        Task<int> FirstIdPersonByNumeroSocio(string numeroSocio, CancellationToken ctk = default);
        Task<int> FirstIdPersonByNumeroTessera(string numeroTessera, CancellationToken ctk = default);
        Task<PersonDTO> FirstPerson(int id, CancellationToken ctk = default);
        Task<PersonDTO> FirstSocio(int idSocio, CancellationToken ctk = default);
        Task<PersonDTO> FirstTessera(int idTessera, CancellationToken ctk = default);
        Task<bool> HasCodiciSocio(int idperson, CancellationToken ctk = default);
        Task<List<PersonDTO>> Load(int id, CancellationToken ctk = default);
        Task<List<PersonDTO>> LoadByCognomeExact(string cognome, CancellationToken ctk = default);
        Task<List<PersonDTO>> LoadByModel(object model, CancellationToken ctk = default);
        Task<List<PersonDTO>> LoadByNatoilExact(int natoil, CancellationToken ctk = default);
        Task<List<PersonDTO>> LoadByNomeExact(string nome, CancellationToken ctk = default);
        Task<List<PersonDTO>> LoadContainsCognome(string cognome, CancellationToken ctk = default);
        Task<List<PersonDTO>> LoadContainsNome(string nome, CancellationToken ctk = default);
        Task<List<PersonDTO>> LoadMaiorNato(int natoil, CancellationToken ctk = default);
        Task<List<PersonDTO>> LoadMinorNato(int natoil, CancellationToken ctk = default);
        Task<List<PersonDTO>> LoadStartByCognome(string cognome, CancellationToken ctk = default);
        Task<List<PersonDTO>> LoadStartByNome(string nome, CancellationToken ctk = default);
        Task<bool> UpdPerson(PersonDTO dto, CancellationToken ctk = default);
        Task<bool> UpdSocio(PersonDTO map, CancellationToken ctk = default);
        Task<bool> UpdTessera(PersonDTO map, CancellationToken ctk = default);
    }
}
