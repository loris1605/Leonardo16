using DTO.Entity;
using Microsoft.EntityFrameworkCore;
using Models.Context;
using Models.Repository;
using Models.Tables;
using System.Linq.Expressions;

namespace DTO.Repository
{
    public interface ISettoreRepository : IBaseRepository<Settore>
    {
        Task<SettoreDTO> FirstSettore(int id, CancellationToken ctk = default);
        Task<List<SettoreDTO>> Load(int id, CancellationToken ctk = default);
        Task<List<SettoreDTO>> LoadSettori(Expression<Func<Settore, bool>> predicate, CancellationToken ctk = default);
        Task<bool> Upd(SettoreDTO dto, CancellationToken ctk = default);
        Task<List<TipoSettoreDTO>> LoadTipiSettore(CancellationToken ctk = default);
        //Task<List<TariffaDTO>> GetListini(int id, CancellationToken ctk = default);
        //Task<bool> SaveListini(int id, List<TariffaDTO> tariffe, CancellationToken ctk = default);
    }

    public class SettoreRepository : BaseRepository<SettoreDbContext, Settore>, ISettoreRepository
    {
        private readonly ISettoreDbContext _ctx;

        public SettoreRepository(ISettoreDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<List<SettoreDTO>> Load(int id, CancellationToken ctk = default)
        {
            if (id > 0)
                return await LoadSettori(x => x.Id == id, ctk);
            else
                return await LoadSettori(p => p.Id > 0, ctk);
        }

        public async Task<List<SettoreDTO>> LoadSettori(Expression<Func<Settore, bool>> predicate
                                                            , CancellationToken ctk = default)
        {
            
            // Carichiamo prima gli operatori con i loro dati (Eager Loading)
            return await _ctx.Settori
                .AsNoTracking()
                .Where(predicate)
                .OrderBy(o => o.Nome)
                .SelectMany(o => o.Listini.DefaultIfEmpty(), SettoreDTO.ToSettoriDtoRelationed) // <--- Usi l'espressione qui
                .ToListAsync(ctk);

        }

        public async Task<List<TipoSettoreDTO>> LoadTipiSettore(CancellationToken ctk = default)
        {
            return await _ctx.TipiSettore
                .AsNoTracking()
                .OrderBy(p => p.Nome)
                .Select(TipoSettoreDTO.ToDto).ToListAsync(ctk);


        }

        public async Task<SettoreDTO> FirstSettore(int id, CancellationToken ctk = default) 
        {
            return await GetById(id, selector: SettoreDTO.ToSettoreDto, ctk: ctk) ?? new SettoreDTO();    
        }

        public async Task<bool> Upd(SettoreDTO dto, CancellationToken ctk = default) => await Upd<SettoreDTO, Settore>(dto, ctk);

        //public async Task<List<TariffaDTO>> GetListini(int id, CancellationToken ctk = default)
        //{
        //    using SettoreDbContext _ctx = new();
        //    return await _ctx.Tariffe
        //        .AsNoTracking()
        //        .Select(TariffaDTO.ToTariffaElencoDto(id)) // Passi l'id qui
        //        .ToListAsync(ctk);
        //}

        //public async Task<bool> SaveListini(int id, List<TariffaDTO> tariffe, CancellationToken ctk = default)
        //{
        //    using SettoreDbContext _ctx = new();
        //    var settore = await _ctx.Settori
        //        .Include(o => o.Listini)
        //        .FirstOrDefaultAsync(o => o.Id == id, ctk);
        //    if (settore == null)
        //        return false;
        //    // Rimuovi i reparti esistenti
        //    _ctx.Listini.RemoveRange(settore.Listini);
        //    // Aggiungi i nuovi reparti
        //    foreach (var tariffadto in tariffe)
        //    {
        //        int tariffaId = tariffadto.Id;
        //        if (tariffadto.HasListino) settore.Listini.Add(new Listino { SettoreId = id, TariffaId = tariffaId });
        //    }
        //    await _ctx.SaveChangesAsync(ctk);
        //    return true;
        //}
    }
}
