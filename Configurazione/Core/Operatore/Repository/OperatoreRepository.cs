using DTO.Entity;
using Microsoft.EntityFrameworkCore;
using Models.Context;
using Models.Repository;
using Models.Tables;
using System.Linq.Expressions;

namespace DTO.Repository
{
    public interface IOperatoreRepository : IBaseRepository<Operatore>
    {
        Task<OperatoreDTO> FirstOperatore(int id, CancellationToken ctk = default);
        Task<List<OperatoreDTO>> Load(int id, CancellationToken ctk = default);
        Task<List<OperatoreDTO>> LoadByModel(object model, CancellationToken ctk = default);
        Task<List<OperatoreDTO>> LoadOperatori(Expression<Func<Operatore, bool>> predicate, CancellationToken ctk = default);
        Task<bool> Upd(OperatoreDTO dto, CancellationToken ctk = default);
        
    }

    public class OperatoreRepository : BaseRepository<OperatoreDbContext, Operatore>, IOperatoreRepository
    {
        private readonly IOperatoreDbContext _ctx;

        public OperatoreRepository(IOperatoreDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<List<OperatoreDTO>> Load(int id, CancellationToken ctk = default)
        {
            if (id > 0)
                return await LoadOperatori(x => x.Id == id, ctk);
            else
                return await LoadOperatori(p => p.Id > -2, ctk);
        }

        public async Task<List<OperatoreDTO>> LoadOperatori(Expression<Func<Operatore, bool>> predicate
                                                            , CancellationToken ctk = default)
        {
            
            // Carichiamo prima gli operatori con i loro dati (Eager Loading)
            var data = await _ctx.Operatori
                .AsNoTracking()
                .Where(predicate)
                .OrderBy(o => o.Nome)
                .SelectMany(o => o.Permessi.DefaultIfEmpty(), OperatoreDTO.ToOperatoriDtoRelationed) // <--- Usi l'espressione qui
                .ToListAsync(ctk);

            return data;

        }

        public async Task<bool> Upd(OperatoreDTO dto, CancellationToken ctk = default) => await Upd<OperatoreDTO, Operatore>(dto, ctk);

        public async Task<OperatoreDTO> FirstOperatore(int id, CancellationToken ctk = default)
        {

            var result = await GetById(id,
                selector: OperatoreDTO.ToOperatoreDto, ctk);
                

            return result ?? new OperatoreDTO();
        }

        


        public async Task<List<OperatoreDTO>> LoadByModel(object model, CancellationToken ctk = default)
        {

            await Task.FromResult(new List<OperatoreDTO>());
            throw new NotImplementedException();
        }

        
    }
}
