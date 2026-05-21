using DTO.Entity;
using Microsoft.EntityFrameworkCore;
using Models.Context;
using Models.Repository;
using Models.Tables;

namespace DTO.Repository
{
    public interface IEntraSocioRepository
    {
        Task<EntraSocioDTO> GetPersonByTessera(string numeroTessera, CancellationToken ctk = default);
        Task<List<EntraIngressiDTO>> GetIngressiByPostazione(int postazioneId, CancellationToken ctk = default);
    }

    public class EntraSocioRepository : BaseRepository<EntraSocioDbContext, Scheda>, IEntraSocioRepository
    {
        private readonly IEntraSocioDbContext _ctx;

        public EntraSocioRepository(IEntraSocioDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<EntraSocioDTO> GetPersonByTessera(string numeroTessera, CancellationToken ctk = default)
        {
            var data = await _ctx.Tessere
                .Where(t => t.NumeroTessera == numeroTessera)
                .Include(t => t.Socio)
                    .ThenInclude(s => s.Person)
                .FirstOrDefaultAsync(ctk);

            if (data == null || data.Socio == null || data.Socio.Person == null) return new EntraSocioDTO();

            return new EntraSocioDTO
            {
                NumeroTessera = data.NumeroTessera,
                CodicePerson = data.Socio.Person.Id,
                Cognome = data.Socio.Person.SurName,
                Nome = data.Socio.Person.FirstName,
                Natoil = data.Socio.Person.Natoil,
                CodiceUnivoco = data.Socio.Person.UniqueParam,
                Blocco = !data.Abilitato,
                NumeroSocio = data.Socio.NumeroSocio,
                ScadenzaTessera = data.Scadenza
            };
        }

        public async Task<List<EntraIngressiDTO>> GetIngressiByPostazione(int postazioneId, CancellationToken ctk = default)
        {
            var data = await _ctx.Tariffe
                .AsNoTracking() // Ottimizza le performance se devi solo mostrare i dati
                .Where(t => t.Listini.Any(l =>
                    l.Settore != null &&
                    l.Settore.TipoSettoreId == -1 &&
                    l.Settore.Reparti.Any(r => r.PostazioneId == postazioneId)
                ))
                .ToListAsync(ctk);

            return data.Select(t => new EntraIngressiDTO
            {
                Id = t.Id, //id tariffa
                NomeTariffa = t.Nome,
                EtichettaTariffa = t.Label,
                PrezzoTariffa = t.Prezzo,
                IsFreeDrink = t.IsFreeDrink
            }).ToList();

        }
    }
}
