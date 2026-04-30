using DTO.Entity;
using Microsoft.EntityFrameworkCore;
using Models.Context;
using Models.Repository;
using Models.Tables;
using System.Linq.Expressions;

namespace DTO.Repository
{
    public interface IPostazioneRepository : IBaseRepository<Postazione>
    {
        Task<PostazioneDTO> FirstPostazione(int id, CancellationToken ctk = default);
        Task<List<PostazioneDTO>> Load(int id, CancellationToken ctk = default);
        Task<List<PostazioneDTO>> LoadPostazioni(Expression<Func<Postazione, bool>> predicate, CancellationToken ctk = default);
        Task<List<TipoPostazioneDTO>> LoadTipiPostazione(CancellationToken ctk = default);
        Task<List<TipoRientroDTO>> LoadTipiRientro(CancellationToken ctk = default);
        Task<bool> Upd(PostazioneDTO dto, CancellationToken ctk = default);
        //Task<bool> SaveReparti(int id, List<SettoreElencoDTO> settori, CancellationToken ctk = default);
        //Task<List<SettoreElencoDTO>> GetReparti(int id, CancellationToken ctk = default);
    }

    public class PostazioneRepository : BaseRepository<PostazioneDbContext, Postazione>, IPostazioneRepository
    {
        private readonly IPostazioneDbContext _ctx;

        public PostazioneRepository(IPostazioneDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<List<PostazioneDTO>> Load(int id, CancellationToken ctk = default)
        {
            if (id > 0)
                return await LoadPostazioni(x => x.Id == id, ctk);
            else
                return await LoadPostazioni(p => p.Id > -2, ctk);
        }

        public async Task<List<PostazioneDTO>> LoadPostazioni(Expression<Func<Postazione, bool>> predicate
                                                            , CancellationToken ctk = default)
        {
            using PostazioneDbContext _ctx = new();

            // Carichiamo prima gli operatori con i loro dati (Eager Loading)
            return await _ctx.Postazioni
                .AsNoTracking()
                .Where(predicate)
                .OrderBy(o => o.Nome)
                .SelectMany(o => o.Reparti.DefaultIfEmpty(), PostazioneDTO.ToPostazioniDtoRelationed) // <--- Usi l'espressione qui
                .ToListAsync(ctk);

        }

        public async Task<PostazioneDTO> FirstPostazione(int id, CancellationToken ctk = default)
        {

            return await GetById(id, selector: PostazioneDTO.ToPostazioneDto, ctk: ctk) ?? new PostazioneDTO();

        }

        public async Task<bool> Upd(PostazioneDTO dto, CancellationToken ctk = default)
        {
            return await Upd<PostazioneDTO, Postazione>(dto, ctk);
        }

        public async Task<List<TipoPostazioneDTO>> LoadTipiPostazione(CancellationToken ctk = default)
        {
            using PostazioneDbContext _ctx = new();
            return await _ctx.TipiPostazione
                .AsNoTracking()
                .OrderBy(p => p.Nome)
                .Select(TipoPostazioneDTO.ToDto).ToListAsync(ctk);
        }

        public async Task<List<TipoRientroDTO>> LoadTipiRientro(CancellationToken ctk = default)
        {
            using PostazioneDbContext _ctx = new();
            return await _ctx.TipiRientro
                .AsNoTracking()
                .OrderBy(p => p.Nome)
                .Select(TipoRientroDTO.ToDto).ToListAsync(ctk);
        }

        //public async Task<List<SettoreElencoDTO>> GetReparti(int id, CancellationToken ctk = default)
        //{
        //    using PostazioneDbContext _ctx = new();
        //    return await _ctx.Settori.AsNoTracking()
        //        .Select(SettoreElencoDTO.ToSettoreElencoDto(id)) // Passi l'id qui
        //        .ToListAsync(ctk);
        //}

        //public async Task<bool> SaveReparti(int id, List<SettoreElencoDTO> settori, CancellationToken ctk = default)
        //{
        //    using PostazioneDbContext _ctx = new();
        //    var postazione = await _ctx.Postazioni
        //        .Include(o => o.Reparti)
        //        .FirstOrDefaultAsync(o => o.Id == id, ctk);
        //    if (postazione == null)
        //        return false;
        //    // Rimuovi i reparti esistenti
        //    _ctx.Reparti.RemoveRange(postazione.Reparti);
        //    // Aggiungi i nuovi reparti
        //    foreach (var settoredto in settori)
        //    {
        //        int settoreId = settoredto.Id;
        //        if (settoredto.HasReparto) postazione.Reparti.Add(new Reparto { SettoreId = settoreId, PostazioneId = id });
        //    }
        //    await _ctx.SaveChangesAsync(ctk);
        //    return true;
        //}
    }
}
