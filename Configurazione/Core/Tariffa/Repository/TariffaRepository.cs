using DTO.Entity;
using Microsoft.EntityFrameworkCore;
using Models.Context;
using Models.Repository;
using Models.Tables;
using System.Linq.Expressions;

namespace DTO.Repository
{
    public interface ITariffaRepository : IBaseRepository<Tariffa>
    {
        Task<TariffaDTO> FirstTariffa(int id);
        Task<List<TariffaDTO>> Load(int id, CancellationToken ctk = default);
        Task<bool> Upd(TariffaDTO dto, CancellationToken ctk = default);
    }

    public class TariffaRepository : BaseRepository<TariffaDbContext, Tariffa>, ITariffaRepository
    {
        private readonly ITariffaDbContext _ctx;

        public TariffaRepository(ITariffaDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<List<TariffaDTO>> Load(int id, CancellationToken ctk = default)
        {
            if (id > 0)
                return await LoadTariffe(x => x.Id == id, ctk);
            else
                return await LoadTariffe(p => p.Id > 0, ctk);
        }

        private async Task<List<TariffaDTO>> LoadTariffe(Expression<Func<Tariffa, bool>> predicate
                                                            , CancellationToken ctk = default)
        {
            
            // Carichiamo prima gli operatori con i loro dati (Eager Loading)
            return await _ctx.Tariffe
                .AsNoTracking()
                .Where(predicate)
                .OrderBy(o => o.Nome)
                .Select(TariffaDTO.ToDto) // <--- Usi l'espressione qui
                .ToListAsync(ctk);

        }

        public async Task<TariffaDTO> FirstTariffa(int id) => await GetById(id, selector: TariffaDTO.ToDto) ?? new TariffaDTO();

        public async Task<bool> Upd(TariffaDTO dto, CancellationToken ctk = default) => await Upd<TariffaDTO, Tariffa>(dto, ctk);
    }
}
