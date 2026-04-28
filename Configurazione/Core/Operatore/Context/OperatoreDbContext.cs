using Microsoft.EntityFrameworkCore;
using Models.Tables;

namespace Models.Context
{
    public interface IOperatoreDbContext
    {
        DbSet<Operatore> Operatori { get; set; }
        DbSet<Permesso> Permessi { get; set; }
        DbSet<Postazione> Postazioni { get; set; }
        DbSet<TipoPostazione> TipiPostazione { get; set; }
    }

    public class OperatoreDbContext : BaseContext, IOperatoreDbContext
    {
        public DbSet<Operatore> Operatori { get; set; } = null!;
        public DbSet<Permesso> Permessi { get; set; } = null!;
        public DbSet<Postazione> Postazioni { get; set; } = null!;
        public DbSet<TipoPostazione> TipiPostazione { get; set; } = null!;
    }
}
