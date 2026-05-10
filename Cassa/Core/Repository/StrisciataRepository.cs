using Microsoft.EntityFrameworkCore;
using Models.Context;
using Models.Repository;
using Models.Tables;
using System.Diagnostics;

namespace DTO.Repository
{
    public interface IStrisciataRepository
    {
        Task DevelopStrisciate(CancellationToken ctk = default);
        Task<List<Strisciata>> GetStrisciate(CancellationToken ctk = default);
    }

    public class StrisciataRepository : BaseRepository<StrisciateDbContext, Strisciata>, IStrisciataRepository
    {
        private readonly IStrisciateDbContext _ctx;

        public StrisciataRepository(IStrisciateDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<List<Strisciata>> GetStrisciate(CancellationToken ctk = default)
        {
            var data = await _ctx.Strisciate.OrderBy(x => x.Id).ToListAsync(ctk);
            return data;
        }

        public async Task DevelopStrisciate(CancellationToken ctk = default)
        {
            var data = await GetStrisciate(ctk);
            if (data.Count == 0) return;

            foreach (Strisciata item in data)
            {
                // 1° caso - Verifichiamo se la UniqueIndentifier esiste già
                string ui = CreateUniqueIdentifier(item);

                if (await EsisteCodiceUnivoco(ui, ctk)) //se il codice univoco esiste già fermati
                {

                    continue; 
                }

                if (!await InsertNewPerson(item, ui, ctk)) continue; // se l'inserimento della persona fallisce fermati
                await DeleteStrisciata(item.Id, ctk);
            }

            await Task.CompletedTask;
        }

        private string CreateUniqueIdentifier(Strisciata record)
        {
            string cognome = (record.Cognome.Trim() ?? "").PadRight(3)[..3]; ;
            string nome = (record.Nome.Trim() ?? "").PadRight(3)[..3]; ;
            string nascita = record.Natoil.ToString();

            return cognome + nome + nascita;
        }

        private async Task<bool> EsisteCodiceUnivoco(string codiceunivoco, CancellationToken ctk = default)
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

        private async Task<bool> InsertNewPerson(Strisciata record, string ui, CancellationToken ctk = default)
        {
            var person = new Person
            {
                FirstName = record.Nome ?? string.Empty,
                SurName = record.Cognome ?? string.Empty,
                Natoil = record.Natoil,
                UniqueParam = ui,

                Soci =
                [
                    new Socio
                    {
                        NumeroSocio = record.CodiceSocio,
                        // Colleghiamo la Tessera direttamente al Socio
                        Tessere =
                        [
                            new Tessera
                            {
                                NumeroTessera = record.NumeroTessera,
                                Scadenza = record.Scadenza
                            }
                        ]
                    }
                ]
            };

            await _ctx.People.AddAsync(person, ctk);

            try
            {
                await _ctx.SaveChangesAsync(ctk);
                return true;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($">>> [ERROR] Add Person: {ex.InnerException?.Message ?? ex.Message}");
                return false;
            }
        }

        private async Task DeleteStrisciata(int id, CancellationToken ctk = default)
        {
            var data = await _ctx.Strisciate.FirstOrDefaultAsync(x => x.Id == id, ctk);

            _ctx.Strisciate.Remove(data);

            try
            {
                await _ctx.SaveChangesAsync(ctk);

            }
            catch (Exception)
            {

            }
        }
    }
}
