using DTO.Entity;
using Microsoft.EntityFrameworkCore;
using Models.Context;
using Models.Entity.Global;
using Models.Repository;
using Models.Tables;
using SysNet;

namespace DTO.Repository
{
    public interface ILoginRepository 
    {
        Task<List<LoginDTO>> GetOperatoriAbilitati(CancellationToken ct = default);
        Task SaveSettings(LoginDTO dto, CancellationToken ct = default);
    }

    public class LoginRepository : BaseRepository<LoginDbContext, Operatore>, ILoginRepository
    {
        private readonly ILoginDbContext _ctx;

        public LoginRepository(ILoginDbContext context)
        {
            _ctx = context;   
        }

        public async Task<List<LoginDTO>> GetOperatoriAbilitati(CancellationToken ctk)
        {
            return await GetAll(
                selector: LoginDTO.ToLoginDto,
                predicate: p => p.Abilitato == true,
                ct : ctk);
        }
      
        private async Task<List<PostazioneXC>> ListPostazioniByOperatore(int CodiceOperatore, CancellationToken ct)
        {
            return await _ctx.Permessi
                        .AsNoTracking()
                        .Where(p => p.OperatoreId == CodiceOperatore)
                        .Select(LoginDTO.ToPostazioneXC)
                        .ToListAsync(ct); // <--- Passiamo il token a EF
        }

        private async Task<List<SettoreXC>> SelectSettoriX(int CodicePostazione, CancellationToken ct)
        {
            return await _ctx.Reparti
                        .AsNoTracking()
                        .Where(p => p.PostazioneId == CodicePostazione)
                        .Select(LoginDTO.ToSettoreXC)
                        .ToListAsync(ct); // <--- Passiamo il token a EF
        }

        private async Task<List<TariffaXC>> SelectTariffeX(int CodiceSettore, CancellationToken ct)
        {
            return await _ctx.Listini
                       .AsNoTracking()
                       .Where(p => p.SettoreId == CodiceSettore)
                       .Select(LoginDTO.ToTariffaXC)
                       .ToListAsync(ct); // <--- Passiamo il token a EF
        }

        public async Task SaveSettings(LoginDTO dT, CancellationToken ct = default)
        {
            // 1. Rimosso Task.Run: le chiamate sotto sono già asincrone.
            // Usiamo il token 'ct' per rendere l'intera catena interrompibile.

            OperatoreXC XOperatore = Create<OperatoreXC>.Instance();

            XOperatore.IDOPERATORE = dT.Id;
            XOperatore.NOMEOPERATORE = dT.NomeOperatore;
            XOperatore.PASSWORD = dT.Password;

            // 2. Passiamo il token a ogni metodo asincrono
            XOperatore.POSTAZIONI = await ListPostazioniByOperatore(dT.Id, ct).ConfigureAwait(false);

            if (XOperatore.POSTAZIONI.Count > 0)
            {
                foreach (var postazione in XOperatore.POSTAZIONI)
                {
                    // 3. Controllo manuale prima di ogni ciclo pesante per massima reattività
                    ct.ThrowIfCancellationRequested();

                    postazione.SETTORI = await SelectSettoriX(postazione.CODICEPOSTAZIONE, ct).ConfigureAwait(false);

                    foreach (var settore in postazione.SETTORI)
                    {
                        // Un controllo rapido anche qui se le query sono molte
                        ct.ThrowIfCancellationRequested();

                        settore.TARIFFE = await SelectTariffeX(settore.CODICESETTORE, ct).ConfigureAwait(false) ?? [];
                    }
                }
            }

            XOperatore.GIORNATA = await GetGiornataOpen(ct).ConfigureAwait(false);

            // 4. Fondamentale: verifichiamo un'ultima volta prima di sovrascrivere le impostazioni globali
            ct.ThrowIfCancellationRequested();

            GlobalValuesC.MySetting = XOperatore;
        }


        private async Task<GiornataXC> GetGiornataOpen(CancellationToken ct)
        {
            return await _ctx.Giornate
                            .AsNoTracking()
                            .Where(x => x.Aperta == true)
                            .Select(LoginDTO.ToGiornataXC)
                            .FirstOrDefaultAsync(ct); // <--- Passiamo il token a EF
        }
    }
}
