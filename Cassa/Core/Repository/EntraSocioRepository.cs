using DTO.Entity;
using Models.Context;
using Models.Repository;
using Models.Tables;

namespace DTO.Repository
{
    public class EntraSocioRepository : BaseRepository<EntraSocioDbContext, Scheda>
    {
        private readonly IEntraSocioDbContext _ctx;

        public EntraSocioRepository(IEntraSocioDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<EntraSocioDTO> GetPersonByTessera(int numeroTessera, CancellationToken ctk = default)
        {
            var data = await _ctx.Tessera
        }

    }
}
