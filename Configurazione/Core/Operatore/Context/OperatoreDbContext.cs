using Microsoft.EntityFrameworkCore;
using Models.Tables;

namespace Models.Context
{
    public class OperatoreDbContext : BaseContext
    {
        public DbSet<Operatore> Operatori { get; set; } = null!;
        public DbSet<Permesso> Permessi { get; set; } = null!;
        public DbSet<Postazione> Postazioni { get; set; } = null!;
        public DbSet<TipoPostazione> TipiPostazione { get; set; } = null!;
    }
}
