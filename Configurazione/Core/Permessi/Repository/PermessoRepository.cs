using DTO.Entity;
using Microsoft.EntityFrameworkCore;
using Models.Context;
using Models.Repository;
using Models.Tables;

namespace DTO.Repository
{
    public interface IPermessoRepository
    {
        Task<List<PostazioneElencoDTO>> GetPermessi(int id, CancellationToken ctk = default);
        Task<bool> SavePermessi(int id, List<PostazioneElencoDTO> postazioni, CancellationToken ctk = default);
    }

    public class PermessoRepository : BaseRepository<PermessoDbContext, Permesso>, IPermessoRepository
    {
        private readonly IPermessoDbContext _ctx;

        public PermessoRepository(IPermessoDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<List<PostazioneElencoDTO>> GetPermessi(int id, CancellationToken ctk = default)
        {
            return await _ctx.Postazioni
                .AsNoTracking()
                .Select(PostazioneElencoDTO.ToPostazioneElencoDto(id)) // Passi l'id qui
                .ToListAsync(ctk);
        }

        public async Task<bool> SavePermessi(int id, List<PostazioneElencoDTO> postazioni, CancellationToken ctk = default)
        {
            var operatore = await _ctx.Operatori
                .Include(o => o.Permessi)
                .FirstOrDefaultAsync(o => o.Id == id, ctk);
            if (operatore == null)
                return false;
            // Rimuovi i permessi esistenti
            _ctx.Permessi.RemoveRange(operatore.Permessi);
            // Aggiungi i nuovi permessi
            foreach (var postazioneid in postazioni)
            {
                int postazioneId = postazioneid.Id;
                if (postazioneid.HasPermesso) operatore.Permessi.Add(new Permesso { PostazioneId = postazioneId, OperatoreId = id });
            }
            await _ctx.SaveChangesAsync(ctk);
            return true;
        }

        public async Task<OperatoreDTO> FirstOperatore(int id, CancellationToken ctk = default)
        {

            var result = await GetById(id,
                selector: OperatoreDTO.ToOperatoreDto, ctk);


            return result ?? new OperatoreDTO();
        }
    }
}
