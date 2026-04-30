using Microsoft.EntityFrameworkCore;
using Models.Tables;

namespace Models.Context
{
    public interface IPermessoDbContext
    {
        DbSet<Permesso> Permessi { get; set; }
        DbSet<Postazione> Postazioni { get; set; }
        DbSet<TipoPostazione> TipiPostazione { get; set; }
        DbSet<Operatore> Operatori { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }

    public class PermessoDbContext : BaseContext, IPermessoDbContext
    {
        public DbSet<Permesso> Permessi { get; set; } = null!;
        public DbSet<Postazione> Postazioni { get; set; } = null!;
        public DbSet<TipoPostazione> TipiPostazione { get; set; } = null!;
        public DbSet<Operatore> Operatori { get; set; } = null!;

    }
}
