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

            if (data == null || data.Socio == null || data.Socio.Person == null) return null;

            return new EntraSocioDTO
            {
                NumeroTessera = data.NumeroTessera,
                CodicePerson = data.Socio.Person.Id,
                Cognome = data.Socio.Person.SurName,
                Nome = data.Socio.Person.FirstName,
                Natoil = data.Socio.Person.Natoil,
                Blocco = !data.Abilitato,
                NumeroSocio = data.Socio.NumeroSocio,
                ScadenzaTessera = data.Scadenza
            };
        }

    }
}
