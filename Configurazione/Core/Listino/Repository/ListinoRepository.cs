using DTO.Entity;
using Microsoft.EntityFrameworkCore;
using Models.Context;
using Models.Repository;
using Models.Tables;

namespace DTO.Repository
{
    public interface IListinoRepository
    {
        Task<List<TariffaDTO>> GetListini(int id, CancellationToken ctk = default);
        Task<bool> SaveListini(int id, List<TariffaDTO> tariffe, CancellationToken ctk = default);
    }

    public class ListinoRepository : BaseRepository<ListinoDbContext, Tariffa>, IListinoRepository
    {
        private readonly IListinoDbContext _ctx;

        public ListinoRepository(IListinoDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<List<TariffaDTO>> GetListini(int id, CancellationToken ctk = default)
        {
            return await _ctx.Tariffe
                .AsNoTracking()
                .Select(TariffaDTO.ToTariffaElencoDto(id)) // Passi l'id qui
                .ToListAsync(ctk);
        }

        public async Task<bool> SaveListini(int id, List<TariffaDTO> tariffe, CancellationToken ctk = default)
        {
            var settore = await _ctx.Settori
                .Include(o => o.Listini)
                .FirstOrDefaultAsync(o => o.Id == id, ctk);
            if (settore == null)
                return false;
            // Rimuovi i reparti esistenti
            _ctx.Listini.RemoveRange(settore.Listini);
            // Aggiungi i nuovi reparti
            foreach (var tariffadto in tariffe)
            {
                int tariffaId = tariffadto.Id;
                if (tariffadto.HasListino) settore.Listini.Add(new Listino { SettoreId = id, TariffaId = tariffaId });
            }
            await _ctx.SaveChangesAsync(ctk);
            return true;
        }
    }
}
