using DTO.Entity;
using Microsoft.EntityFrameworkCore;
using Models.Context;
using Models.Repository;
using Models.Tables;
using System.Diagnostics;
using System.Linq.Expressions;

namespace DTO.Repository
{
    public interface IPersonRepository : IBaseRepository<Person>
    {
        Task<int> AddCodiceSocio(PersonDTO map, CancellationToken ctk = default);
        Task<int> AddPerson(PersonDTO map, CancellationToken ctk=default);
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

    public class PersonRepository : BaseRepository<PeopleDbContext, Person>, IPersonRepository
    {
        private readonly IPeopleDbContext _ctx;

        public PersonRepository(IPeopleDbContext ctx)
        {
            _ctx = ctx;
        }

        //da controllare
        public Task<List<PersonDTO>> LoadByModel(object model, CancellationToken ctk = default)
        {
            if (ctk.IsCancellationRequested)
                return Task.FromCanceled<List<PersonDTO>>(ctk);

            return Task.FromResult((List<PersonDTO>)model);
        }

        private async Task<List<PersonDTO>> LoadPeople(Expression<Func<Person, bool>> predicate,
                                              CancellationToken ctk = default)
        {
            var query = _ctx.People
                .AsNoTracking()
                .Where(predicate)
                .SelectMany(
                    person => person.Soci.DefaultIfEmpty(),
                    (person, socio) => new { person, socio })
                .SelectMany(
                    combined => combined.socio!.Tessere.DefaultIfEmpty(),
                    (combined, tessera) => new { combined.person, combined.socio, tessera });

            return await query
                .Select(x => new PersonDTO
                {
                    Id = x.person.Id,
                    Cognome = x.person.SurName ?? string.Empty,
                    Nome = x.person.FirstName ?? string.Empty,
                    Natoil = x.person.Natoil,
                    CodiceUnivoco = x.person.UniqueParam ?? string.Empty, // Aggiunto questo

                    CodiceSocio = x.socio != null ? x.socio.Id : 0,
                    NumeroSocio = (x.socio != null && x.socio.NumeroSocio != null) ? x.socio.NumeroSocio : string.Empty,

                    CodiceTessera = x.tessera != null ? x.tessera.Id : 0,
                    NumeroTessera = (x.tessera != null && x.tessera.NumeroTessera != null) ? x.tessera.NumeroTessera : string.Empty,
                    Scadenza = x.tessera != null ? x.tessera.Scadenza : 0
                })
                .OrderBy(o => o.Cognome)
                .ThenBy(o => o.Nome) // Consiglio: aggiungi il secondo ordinamento
                .Take(100)
                .ToListAsync(ctk);

        }


        public async Task<List<PersonDTO>> Load(int id, CancellationToken ctk = default)
        {
            //IQueryable<Person> query = _ctx.People.AsNoTracking();

            if (id > 0)
                return await LoadPeople(x => x.Id == id, ctk);
            else
                return await LoadPeople(p => p.Id > 0, ctk);

        }

        public async Task<List<PersonDTO>> LoadByCognomeExact(string cognome, CancellationToken ctk = default) =>
                  await LoadPeople(p => p.SurName == cognome, ctk);

        public async Task<List<PersonDTO>> LoadStartByCognome(string cognome, CancellationToken ctk = default) =>
                   await LoadPeople(p => p.SurName.StartsWith(cognome), ctk);

        public async Task<List<PersonDTO>> LoadContainsCognome(string cognome, CancellationToken ctk = default) =>
                   await LoadPeople(p => p.SurName.Contains(cognome), ctk);

        public async Task<List<PersonDTO>> LoadByNomeExact(string nome, CancellationToken ctk = default) =>
                   await LoadPeople(p => p.FirstName == nome, ctk);

        public async Task<List<PersonDTO>> LoadStartByNome(string nome, CancellationToken ctk = default) =>
                   await LoadPeople(p => p.FirstName.StartsWith(nome), ctk);

        public async Task<List<PersonDTO>> LoadContainsNome(string nome, CancellationToken ctk = default) =>
                   await LoadPeople(p => p.FirstName.Contains(nome), ctk);

        public async Task<List<PersonDTO>> LoadByNatoilExact(int natoil, CancellationToken ctk = default) =>
                   await LoadPeople(p => p.Natoil == natoil, ctk);

        public async Task<List<PersonDTO>> LoadMinorNato(int natoil, CancellationToken ctk = default) =>
                   await LoadPeople(p => p.Natoil <= natoil, ctk);

        public async Task<List<PersonDTO>> LoadMaiorNato(int natoil, CancellationToken ctk = default) =>
                   await LoadPeople(p => p.Natoil >= natoil, ctk);

        public async Task<PersonDTO> FirstPerson(int id, CancellationToken ctk = default)
        {
            var result = await _ctx.People
                .AsNoTracking()
                .Where(p => p.Id == id)
                .Select(p => new PersonDTO // Proiezione esplicita (Traducibile in SQL)
                {
                    Id = p.Id,
                    Nome = p.FirstName ?? string.Empty,
                    Cognome = p.SurName ?? string.Empty,
                    Natoil = p.Natoil,
                    CodiceUnivoco = p.UniqueParam ?? string.Empty,

                    // Aggiungi qui solo i campi necessari per il "Simple" DTO
                })
                .FirstOrDefaultAsync(ctk);

            ctk.ThrowIfCancellationRequested();

            return result ?? new PersonDTO();
        }

        public async Task<PersonDTO> FirstSocio(int idSocio, CancellationToken ctk = default)
        {
            var result = await _ctx.Soci
                .AsNoTracking()
                .Where(s => s.Id == idSocio)
                // Partiamo dal Socio (s), carichiamo la Persona (s.Person) 
                // e le Tessere (s.Tessere) con LEFT JOIN
                .Select(p => new PersonDTO // Proiezione esplicita (Traducibile in SQL)
                {
                    Id = p.PersonId,
                    Nome = p.Person!.FirstName ?? string.Empty,
                    Cognome = p.Person!.SurName ?? string.Empty,
                    Natoil = p.Person!.Natoil,
                    CodiceUnivoco = p.Person!.UniqueParam ?? string.Empty,
                    CodiceSocio = p.Id,
                    NumeroSocio = p.NumeroSocio

                    // Aggiungi qui solo i campi necessari per il "Simple" DTO
                })
                .FirstOrDefaultAsync(ctk);

            ctk.ThrowIfCancellationRequested();

            // Se non trova il socio, restituisce un oggetto vuoto per il binding
            return result ?? new PersonDTO();
        }

        public async Task<PersonDTO> FirstTessera(int idTessera, CancellationToken ctk = default)
        {
            var result = await _ctx.Tessere
                .AsNoTracking()
                .Where(t => t.Id == idTessera)
                // Risaliamo alla Persona tramite il Socio (t.Socio.Person)
                // Passiamo: (Persona, Socio, Tessera)
                .Select(p => new PersonDTO
                {
                    Id = p.Socio!.PersonId,
                    Nome = p.Socio!.Person!.FirstName ?? string.Empty,
                    Cognome = p.Socio!.Person!.SurName ?? string.Empty,
                    Natoil = p.Socio!.Person!.Natoil,
                    CodiceUnivoco = p.Socio!.Person!.UniqueParam ?? string.Empty,
                    CodiceSocio = p.SocioId,
                    NumeroSocio = p.Socio!.NumeroSocio ?? string.Empty,
                    CodiceTessera = p.Id,
                    NumeroTessera = p.NumeroTessera,
                    Scadenza = p.Scadenza

                })
                .FirstOrDefaultAsync(ctk);

            ctk.ThrowIfCancellationRequested();

            // Ritorna il record completo (Persona + Socio + Dati di QUESTA tessera)
            return result ?? new PersonDTO();
        }

        public async Task<int> FirstIdPersonByNumeroTessera(string numeroTessera, CancellationToken ctk = default)
        {
            var result = await _ctx.Tessere
                .AsNoTracking()
                .Where(t => t.NumeroTessera == numeroTessera)
                .Select(t => t.Socio!.PersonId)
                .FirstOrDefaultAsync(ctk);

            ctk.ThrowIfCancellationRequested();

            return result; // Se non trova nulla, ritorna 0 (default int)
        }

        public async Task<int> FirstIdPersonByNumeroSocio(string numeroSocio, CancellationToken ctk = default)
        {
            var result = await _ctx.Soci
                .AsNoTracking()
                .Where(s => s.NumeroSocio == numeroSocio)
                .Select(s => s.PersonId)
                .FirstOrDefaultAsync(ctk);

            ctk.ThrowIfCancellationRequested();
            return result; // Se non trova nulla, ritorna 0 (default int)
        }

        public async Task<bool> HasCodiciSocio(int idperson, CancellationToken ctk = default)
        {
            var result = await _ctx.Soci.AnyAsync(s => s.PersonId == idperson, ctk);

            return result;

        }

        public async Task<int> AddPerson(PersonDTO map, CancellationToken ctk = default)
        {
            var person = new Person
            {
                FirstName = map.Nome ?? string.Empty,
                SurName = map.Cognome ?? string.Empty,
                Natoil = map.Natoil,
                UniqueParam = map.CodiceUnivoco,


                // Colleghiamo il Socio direttamente alla lista della Persona
                Soci =
                [
                    new Socio
                    {
                        NumeroSocio = map.NumeroSocio,
                        // Colleghiamo la Tessera direttamente al Socio
                        Tessere =
                        [
                            new Tessera
                            {
                                NumeroTessera = map.NumeroTessera,
                                Scadenza = map.Scadenza
                            }
                        ]
                    }
                ]
            };

            // 2. Aggiungiamo solo la "radice" (Person). EF aggiungerà i figli a cascata.
            await _ctx.People.AddAsync(person, ctk);

            try
            {
                await _ctx.SaveChangesAsync(ctk);
                return person.Id;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($">>> [ERROR] Add Person: {ex.InnerException?.Message ?? ex.Message}");
                return -1;
            }
        }
        public async Task<int> AddCodiceSocio(PersonDTO map, CancellationToken ctk = default)
        {
            ctk.ThrowIfCancellationRequested();

            var socio = new Socio
            {
                NumeroSocio = map.NumeroSocio,
                PersonId = map.Id, // Assumiamo che map.Id sia già valorizzato con l'ID della Person esistente
                // Colleghiamo la Tessera direttamente al Socio
                Tessere =
                [
                    new Tessera
                    {
                        NumeroTessera = map.NumeroTessera,
                        Scadenza = map.Scadenza
                    }
                ]

            };

            // 2. Aggiungiamo solo la "radice" (Person). EF aggiungerà i figli a cascata.
            await _ctx.Soci.AddAsync(socio, ctk);

            try
            {
                await _ctx.SaveChangesAsync(ctk);
                return socio.Id;
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine(">>> [INFO] Inserimento Codice Socio annullato dall'utente.");
                return -1;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($">>> [ERROR] Add: {ex.InnerException?.Message ?? ex.Message}");
                return -1;
            }

        }
        public async Task<int> AddTessera(PersonDTO map, CancellationToken ctk = default)
        {
            ctk.ThrowIfCancellationRequested();
            
            if (map.CodiceSocio <= 0) return -1;

            var tessera = new Tessera
            {
                NumeroTessera = map.NumeroTessera,
                SocioId = map.CodiceSocio,
                Abilitato = true

            };

            await _ctx.Tessere.AddAsync(tessera, ctk);

            try
            {
                await _ctx.SaveChangesAsync(ctk);
                return tessera.Id;
            }
            catch (OperationCanceledException)
            {
                // Rilanciamo l'eccezione per far capire al ViewModel che è stato l'utente ad annullare
                throw;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($">>> [ERROR] Add: {ex.InnerException?.Message ?? ex.Message}");
                return -1;
            }

        }

        public async Task<bool> UpdPerson(PersonDTO dto, CancellationToken ctk = default)
        {
            return await Upd<PersonDTO, Person>(dto, ctk);
        }

        public async Task<bool> UpdSocio(PersonDTO map, CancellationToken ctk = default)
        {
            ctk.ThrowIfCancellationRequested();
            // 1. Cerchiamo l'entità (FindAsync è ottimo qui)
            var socio = await _ctx.Soci.FindAsync(new object[] { map.CodiceSocio, ctk }, cancellationToken: ctk);
            if (socio == null) return false;
            // 2. Aggiorniamo le proprietà
            socio.NumeroSocio = map.NumeroSocio;

            ctk.ThrowIfCancellationRequested();

            try
            {
                await _ctx.SaveChangesAsync(ctk);
                return true;
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine(">>> [INFO] Inserimento Socio annullato dall'utente.");
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($">>> [ERROR] Add: {ex.InnerException?.Message ?? ex.Message}");
                return false;
            }

        }
        public async Task<bool> UpdTessera(PersonDTO map, CancellationToken ctk = default)
        {
            ctk.ThrowIfCancellationRequested();

            // 1. Cerchiamo l'entità (FindAsync è ottimo qui)
            var tessera = await _ctx.Tessere.FindAsync(new object[] { map.CodiceTessera, ctk }, cancellationToken: ctk);
            if (tessera == null) return false;
            // 2. Aggiorniamo le proprietà
            tessera.NumeroTessera = map.NumeroTessera;

            try
            {
                await _ctx.SaveChangesAsync(ctk);
                return true;
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine(">>> [INFO] Inserimento Tessera annullato dall'utente.");
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($">>> [ERROR] Add: {ex.InnerException?.Message ?? ex.Message}");
                return false;
            }

        }

        public async Task<bool> DelSocio(PersonDTO map, CancellationToken ctk = default)
        {
            ctk.ThrowIfCancellationRequested();
            var socio = await _ctx.Soci.FindAsync(new object[] { map.CodiceSocio, ctk }, cancellationToken: ctk);
            if (socio == null) return false;

            _ctx.Soci.Remove(socio);

            ctk.ThrowIfCancellationRequested();

            try
            {
                await _ctx.SaveChangesAsync(ctk);
                return true;
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine(">>> [INFO] Cancellazioe Socio annullato dall'utente.");
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($">>> [ERROR] Add: {ex.InnerException?.Message ?? ex.Message}");
                return false;
            }
        }
        public async Task<bool> DelTessera(PersonDTO map, CancellationToken ctk = default)
        {
            ctk.ThrowIfCancellationRequested();
            var tessera = await _ctx.Tessere.FindAsync(new object[] { map.CodiceTessera, ctk }, cancellationToken: ctk);
            if (tessera == null) return false;

            _ctx.Tessere.Remove(tessera);

            try
            {
                await _ctx.SaveChangesAsync(ctk);
                return true;
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine(">>> [INFO] Cancellazioe Tessera annullato dall'utente.");
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($">>> [ERROR] Add: {ex.InnerException?.Message ?? ex.Message}");
                return false;
            }
        }

        public async Task<bool> EsisteCodiceUnivoco(string codiceunivoco, CancellationToken ctk = default)
        {
            ctk.ThrowIfCancellationRequested();
            

            try
            {
                var result = await _ctx.People.AnyAsync(p => p.UniqueParam == codiceunivoco, ctk);
                return result;
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine(">>> [INFO] Operazione annullata dall'utente.");
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($">>> [ERROR] Add: {ex.InnerException?.Message ?? ex.Message}");
                return false;
            }
        }
        public async Task<bool> EsisteCodiceUnivoco(string codiceunivoco, int id, CancellationToken ctk = default)
        {
            ctk.ThrowIfCancellationRequested();
            try
            {
                var result = await _ctx.People.AnyAsync(p => p.UniqueParam == codiceunivoco && p.Id != id, ctk);
                return result;
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine(">>> [INFO] Operazione annullata dall'utente.");
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($">>> [ERROR] Add: {ex.InnerException?.Message ?? ex.Message}");
                return false;
            }

        }

        public async Task<bool> EsisteNumeroTessera(string numeroTessera, CancellationToken ctk = default)
        {
            ctk.ThrowIfCancellationRequested();
            
            try
            {
                var result = await _ctx.Tessere.AnyAsync(t => t.NumeroTessera == numeroTessera, ctk);
                return result;
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine(">>> [INFO] Operazione annullata dall'utente.");
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($">>> [ERROR] Add: {ex.InnerException?.Message ?? ex.Message}");
                return false;
            }

        }
        public async Task<bool> EsisteNumeroTesseraUpd(PersonDTO dT, CancellationToken ctk = default)
        {
            ctk.ThrowIfCancellationRequested();
            
            try
            {
                var result = await _ctx.Tessere.AnyAsync(t => t.NumeroTessera == dT.NumeroTessera &&
                                                    t.Id != dT.CodiceTessera, ctk);
                return result;
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine(">>> [INFO] Operazione annullata dall'utente.");
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($">>> [ERROR] Add: {ex.InnerException?.Message ?? ex.Message}");
                return false;
            }

        }

        public async Task<bool> EsisteNumeroSocio(string numeroSocio, CancellationToken ctk = default)
        {
            ctk.ThrowIfCancellationRequested();
            
            try
            {
                var result = await _ctx.Soci.AnyAsync(s => s.NumeroSocio == numeroSocio, ctk);
                return result;
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine(">>> [INFO] Operazione annullata dall'utente.");
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($">>> [ERROR] Add: {ex.InnerException?.Message ?? ex.Message}");
                return false;
            }

        }
        public async Task<bool> EsisteNumeroSocioUpd(PersonDTO dT, CancellationToken ctk = default)
        {
            ctk.ThrowIfCancellationRequested();
             
            try
            {
                var result = await _ctx.Soci.AnyAsync(s => s.NumeroSocio == dT.NumeroSocio &&
                                                 s.Id != dT.CodiceSocio, ctk);
                return result;
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine(">>> [INFO] Operazione annullata dall'utente.");
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($">>> [ERROR] Add: {ex.InnerException?.Message ?? ex.Message}");
                return false;
            }

        }

    }
}
