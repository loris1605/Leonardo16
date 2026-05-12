using Microsoft.EntityFrameworkCore;
using Models.Context;
using Models.Repository;
using Models.Tables;
using System;
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

            var uis = data.Select(s => CreateUniqueIdentifier(s)).ToList();

            // Controlliamo quali esistono già sul DB con una sola query
            var existingOnDb = await _ctx.People
                .Where(p => uis.Contains(p.UniqueParam))
                .Select(p => p.UniqueParam)
                .ToListAsync(ctk);


            foreach (Strisciata item in data)
            {
                string ui = CreateUniqueIdentifier(item);

                if (existingOnDb.Contains(ui))
                {
                    var existingPerson = await _ctx.People
                                            .Include(p => p.Soci)
                                                .ThenInclude(s => s.Tessere)
                                            .FirstOrDefaultAsync(p => p.UniqueParam == ui, ctk);

                    if (existingPerson != null)
                    {
                        // 1. Aggiornamento Anagrafica
                        existingPerson.FirstName = item.Nome ?? string.Empty;
                        existingPerson.SurName = item.Cognome ?? string.Empty;
                        existingPerson.Natoil = item.Natoil;

                        // 2. Gestione Socio
                        var socioEsistente = existingPerson.Soci.FirstOrDefault(s => s.NumeroSocio == item.CodiceSocio);

                        if (socioEsistente == null)
                        {
                            existingPerson.Soci.Add(new Socio
                            {
                                NumeroSocio = item.CodiceSocio,
                                Tessere = new List<Tessera>
                    {
                        new Tessera { NumeroTessera = item.NumeroTessera, Scadenza = item.Scadenza }
                    }
                            });
                        }
                        else
                        {
                            // 3. Gestione Tessera
                            var tesseraEsistente = socioEsistente.Tessere
                                .FirstOrDefault(t => t.NumeroTessera == item.NumeroTessera);

                            if (tesseraEsistente == null)
                            {
                                socioEsistente.Tessere.Add(new Tessera
                                {
                                    NumeroTessera = item.NumeroTessera,
                                    Scadenza = item.Scadenza
                                });
                            }
                            else
                            {
                                tesseraEsistente.Scadenza = item.Scadenza;
                            }
                        }
                        _ctx.People.Update(existingPerson);
                    }
                }
                else
                {
                    // Se non esiste, inseriamo tutto il nuovo grafo
                    await _ctx.People.AddAsync(MapToPersonForInsert(item, ui), ctk);

                    // Importante: aggiorniamo la lista locale per evitare duplicati se nel batch 
                    // ci sono 2 record con lo stesso UI nuovo
                    existingOnDb.Add(ui);
                }

                // Rimuoviamo la strisciata in entrambi i casi (aggiornamento o inserimento)
                _ctx.Strisciate.Remove(item);
            }

            // Fuori dal ciclo salviamo tutto una volta sola
            try
            {
                await _ctx.SaveChangesAsync(ctk);
                return ;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($">>> [ERROR] Add Person: {ex.InnerException?.Message ?? ex.Message}");
                return ;
            }

        }

        private string CreateUniqueIdentifier(Strisciata record)
        {
            // Usa "" se null, poi fai Trim
            string cognome = (record.Cognome ?? "").Trim().PadRight(3)[..3];
            string nome = (record.Nome ?? "").Trim().PadRight(3)[..3];
            string nascita = record.Natoil.ToString("yyyyMMdd"); // Formato fisso essenziale!

            return (cognome + nome + nascita).ToUpper(); // ToUpper aiuta a evitare problemi di case-sensitivity
        }

        private Person MapToPersonForInsert(Strisciata record, string uniqueIdentifier)
        {
            return new Person
            {
                FirstName = record.Nome ?? string.Empty,
                SurName = record.Cognome ?? string.Empty,
                Natoil = record.Natoil,
                UniqueParam = uniqueIdentifier,
                Soci = new List<Socio>
                {
                    new Socio
                    {
                        NumeroSocio = record.CodiceSocio,
                        Tessere = new List<Tessera>
                        {
                            new Tessera
                            {
                                NumeroTessera = record.NumeroTessera,
                                Scadenza = record.Scadenza
                            }
                        }
                    }
                }
            };
        }

        private async Task<bool> EsisteCodiceUnivoco(string codiceunivoco, CancellationToken ctk = default)
        {
            ctk.ThrowIfCancellationRequested();


            try
            {
                var result = await _ctx.People.AsNoTracking().AnyAsync(p => p.UniqueParam == codiceunivoco, ctk);
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

        private async Task UpdateAnagrafica(Strisciata record, string ui, CancellationToken ctk = default)
        {
            ctk.ThrowIfCancellationRequested();

            try
            {
                var result = await _ctx.People.Where(p => p.UniqueParam == ui).FirstOrDefaultAsync(ctk);
                if (result == null) return;

                result.SurName = record.Cognome;
                result.FirstName = record.Nome;
                result.Natoil = record.Natoil;

                await _ctx.SaveChangesAsync(ctk);

                return;
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine(">>> [INFO] Operazione annullata dall'utente.");
                return;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($">>> [ERROR] Add: {ex.InnerException?.Message ?? ex.Message}");
                return;
            }
        }
    }
}
