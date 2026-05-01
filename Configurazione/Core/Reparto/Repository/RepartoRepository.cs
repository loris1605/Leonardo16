using DTO.Entity;
using Microsoft.EntityFrameworkCore;
using Models.Context;
using Models.Repository;
using Models.Tables;

namespace DTO.Repository
{
    public interface IRepartoRepository
    {
        Task<List<SettoreElencoDTO>> GetReparti(int id, CancellationToken ctk = default);
        Task<bool> SaveReparti(int id, List<SettoreElencoDTO> settori, CancellationToken ctk = default);
    }

    public class RepartoRepository : BaseRepository<RepartoDbContext, Reparto>, IRepartoRepository
    {
        private readonly IRepartoDbContext _ctx;

        public RepartoRepository(IRepartoDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<List<SettoreElencoDTO>> GetReparti(int id, CancellationToken ctk = default)
        {
            return await _ctx.Settori.AsNoTracking()
                .Select(SettoreElencoDTO.ToSettoreElencoDto(id)) // Passi l'id qui
                .ToListAsync(ctk);
        }

        public async Task<bool> SaveReparti(int id, List<SettoreElencoDTO> settori, CancellationToken ctk = default)
        {
            var postazione = await _ctx.Postazioni
                .Include(o => o.Reparti)
                .FirstOrDefaultAsync(o => o.Id == id, ctk);
            if (postazione == null)
                return false;
            // Rimuovi i reparti esistenti
            _ctx.Reparti.RemoveRange(postazione.Reparti);
            // Aggiungi i nuovi reparti
            foreach (var settoredto in settori)
            {
                int settoreId = settoredto.Id;
                if (settoredto.HasReparto) postazione.Reparti.Add(new Reparto { SettoreId = settoreId, PostazioneId = id });
            }
            await _ctx.SaveChangesAsync(ctk);
            return true;
        }
    }
}
