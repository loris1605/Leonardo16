using Microsoft.EntityFrameworkCore;
using Models.Interfaces;
using Models.Tables;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Models.Repository
{
    public interface IBaseRepository<Ttable> where Ttable : class, IStandardTable, new()
    {
        Task<int> Add<TMap>(TMap map, CancellationToken ctk = default) where TMap : IMappable<Ttable>;
        Task<bool> Del(IMap map, CancellationToken ctk = default);
        void Dispose();
        Task<bool> EsisteNome(IMap dT, CancellationToken ctk = default);
        Task<bool> EsisteNomeUpd(IMap dT, CancellationToken ctk = default);
        Task<List<TMap>> GetAll<TMap>(Expression<Func<Ttable, TMap>> selector, Expression<Func<Ttable, bool>>? predicate = null, Expression<Func<Ttable, object>>? orderBy = null) where TMap : class, new();
        Task<List<TResult>> GetAll<TResult>(Expression<Func<Ttable, TResult>> selector, Expression<Func<Ttable, bool>>? predicate = null, CancellationToken ct = default);
        Task<TMap> GetById<TMap>(int id, Expression<Func<Ttable, TMap>> selector, CancellationToken ctk = default) where TMap : class, new();
        Task<bool> Upd<Tdto, DbTable>(Tdto dto, CancellationToken ctk = default)
                            where Tdto : IMappable<DbTable>, IMap
                            where DbTable : class, new();
    }

    public abstract class BaseRepository<TContext, Ttable> : IBaseRepository<Ttable> where TContext : DbContext, new()
                                    where Ttable : class, IStandardTable, new()
    {

        public BaseRepository()
        {

            Debug.WriteLine($"***** [GC] {this.GetType().Name} {this.GetHashCode()} CARICATO *****");

        }

#if DEBUG
        ~BaseRepository()
        {
            // Questo apparirà nella finestra "Output" di Visual Studio
            Debug.WriteLine($"***** [GC] {this.GetType().Name} {this.GetHashCode()} DISTRUTTO *****");
        }
#endif


        public async Task<bool> EsisteNome(IMap dT, CancellationToken ctk = default)
        {
            using TContext _ctx = new();
            return await _ctx.Set<Ttable>().AnyAsync(p => p.Nome == dT.Nome, ctk);
        }

        public async Task<List<TResult>> GetAll<TResult>(
                        Expression<Func<Ttable, TResult>> selector, // Necessario per OperatoreMapper.ToLoginDto
                        Expression<Func<Ttable, bool>>? predicate = null,
                        CancellationToken ct = default)
        {
            using TContext _ctx = new();
            IQueryable<Ttable> query = _ctx.Set<Ttable>().AsNoTracking();

            if (predicate != null)
                query = query.Where(predicate);

            return await query.Select(selector).ToListAsync(ct);
        }

        public async Task<bool> EsisteNomeUpd(IMap dT, CancellationToken ctk = default)
        {
            using TContext _ctx = new();
            return await _ctx.Set<Ttable>().AnyAsync(p => p.Nome == dT.Nome && p.Id != dT.Id, ctk);
        }

        public async Task<bool> Del(IMap map, CancellationToken ctk = default)
        {
            using TContext _ctx = new();
            var row = await _ctx.Set<Ttable>().FindAsync(map.Id, ctk);

            if (row == null) return false;

            _ctx.Set<Ttable>().Remove(row);

            try
            {
                // 3. Applichiamo la modifica al DB
                await _ctx.SaveChangesAsync(ctk);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Errore Delete: {ex.Message}");
                return false;
            }
        }

        public virtual async Task<int> Add<TMap>(TMap map, CancellationToken ctk = default)
                            where TMap : IMappable<Ttable> // Accetta solo mappe che sanno convertirsi
        {
            using TContext _ctx = new();
            var entity = map.ToTable(); // Chiama il tuo mapper manuale
            await _ctx.Set<Ttable>().AddAsync(entity, ctk);
            try
            {
                // 3. Un'unica chiamata al database (Transazione atomica)
                await _ctx.SaveChangesAsync(ctk);

                // Dopo SaveChanges, person.Id contiene l'ID reale generato dal DB
                return entity.Id;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Errore Add: {ex.InnerException?.Message ?? ex.Message}");
                return -1;
            }
        }

        public async Task<bool> Upd<Tdto,DbTable>(Tdto dto, CancellationToken ctk = default)
                            where Tdto : IMappable<DbTable>, IMap
                            where DbTable : class, new()
        {
            using TContext _ctx = new();
            DbTable? entity = await _ctx.Set<DbTable>().FindAsync(dto.Id, ctk);

            if (entity is null)
            {
                return false; // Oppure lancia un'eccezione specifica
            }

            dto.UpdateTable(entity);

            await _ctx.SaveChangesAsync(ctk);
            return true;

        }
       


        public virtual async Task<TMap> GetById<TMap>(int id, Expression<Func<Ttable, TMap>> selector,
                                                      CancellationToken ctk = default)
                                            where TMap : class, new()
        {
            using TContext _ctx = new();

            // Usiamo la proiezione (selector) direttamente nella query SQL
            var result = await _ctx.Set<Ttable>()
                                   .AsNoTracking()
                                   .Where(p => p.Id == id)
                                   .Select(selector)
                                   .FirstOrDefaultAsync(ctk);

            // Se non trova nulla, restituisce una nuova istanza pulita
            return result ?? new TMap();
        }

        public async Task<List<TMap>> GetAll<TMap>(Expression<Func<Ttable, TMap>> selector,
                                                   Expression<Func<Ttable, bool>>? predicate = null,
                                                   Expression<Func<Ttable, object>>? orderBy = null)
                                            where TMap : class, new()
        {
            using TContext _ctx = new();

            IQueryable<Ttable> query = _ctx.Set<Ttable>().AsNoTracking();

            if (predicate is not null)
            {
                query = query.Where(predicate);
            }

            if (orderBy is not null)
            {
                query = query.OrderBy(orderBy);
            }

            return await query.Select(selector).ToListAsync();
        }

        public virtual void Dispose()
        {
#if DEBUG
            Debug.WriteLine($"***** [VM] {this.GetType().Name} {this.GetHashCode()} disposed *****");
#endif
        }

    }
}
