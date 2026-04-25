using DTO.Entity;
using Menu.Core.DTO;
using Microsoft.EntityFrameworkCore;
using Models.Context;
using Models.Repository;
using Models.Tables;
using System.Diagnostics;

namespace DTO.Repository
{
    public interface IMenuRepository
    {
        Task<List<MenuDTO>> CaricaPostazioniCassa(int CodiceOperatore, CancellationToken ctk = default);
        Task<bool> EsisteGiornataAperta(CancellationToken ctk = default);
        bool OpenGiornata(CancellationToken ctk = default);

    }

    public class MenuRepository : BaseRepository<MenuDbContext, Permesso>, IMenuRepository
    {
        public async Task<bool> EsisteGiornataAperta(CancellationToken ctk = default)
        {
            ctk.ThrowIfCancellationRequested();
            using MenuDbContext _ctx = new();

            try
            {
                var result = await _ctx.Giornate.AnyAsync(p => p.Aperta, ctk);
                return result;
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine(">>> [INFO] Operazione annullata dall'utente.");
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($">>> [ERROR] Add: {ex.InnerException?.Message ?? ex.Message}");
                return false;
            }
            
        }

        public async Task<List<MenuDTO>> CaricaPostazioniCassa(int CodiceOperatore, CancellationToken ctk = default)
        {
            ctk.ThrowIfCancellationRequested();

            using MenuDbContext _ctx = new();
            IQueryable<Permesso> query =
                _ctx.Permessi
                    .AsNoTracking()
                    .Where(p => p.OperatoreId == CodiceOperatore)
                    .Where(p => p.Postazione!.TipoPostazioneId == 2)
                    .Where(p => p.PostazioneId > 0);

            try
            {
                var result = await query.Select(MenuDTO.ToPermessoDTO).ToListAsync(ctk);
                return result;
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine(">>> [INFO] Operazione annullata dall'utente.");
                return new List<MenuDTO>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($">>> [ERROR] Add: {ex.InnerException?.Message ?? ex.Message}");
                return new List<MenuDTO>();
            }

        }

        public bool OpenGiornata(CancellationToken ctk = default)
        {

            using MenuDbContext _ctx = new();
            try
            {
                var giornata = new Giornata
                {
                    Aperta = true,
                    DataInizio = DateTime.Now,
                    DataFine = DateTime.MaxValue
                };

                _ctx.Giornate.Add(giornata);
                _ctx.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($">>> [ERROR] OpenGiornata: {ex.InnerException?.Message ?? ex.Message}");
                return false;

            }
        }
    }
}
