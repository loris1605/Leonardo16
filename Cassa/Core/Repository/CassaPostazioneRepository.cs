using Microsoft.EntityFrameworkCore;
using Models.Context;
using Models.Repository;
using Models.Tables;

namespace DTO.Repository
{
    public interface ICassaPostazioneRepository
    {
        Task<string> GetPostazioneName(int id, CancellationToken ctk = default);
    }

    public class CassaPostazioneRepository : BaseRepository<CassaPostazioneDbContext, Scheda>, ICassaPostazioneRepository
    {
        private readonly ICassaPostazioneDbContext _ctx;

        public CassaPostazioneRepository(ICassaPostazioneDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<string> GetPostazioneName(int id, CancellationToken ctk = default)
        {
            var result = await _ctx.Postazioni.Where(x => x.Id == id).FirstOrDefaultAsync(ctk);
            return result.Nome;
        }
    }

}
