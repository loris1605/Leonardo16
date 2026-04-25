using Microsoft.EntityFrameworkCore;
using Models.Tables;

namespace Models.Context
{
    public class SettoreDbContext : BaseContext
    {
        public DbSet<Settore> Settori { get; set; } = null!;
        public DbSet<Listino> Listini { get; set; } = null!;
        public DbSet<TipoSettore> TipiSettore { get; set; } = null!;
        public DbSet<Tariffa> Tariffe { get; set; } = null!;
        public DbSet<Reparto> Reparti { get; set; } = null!;
    }
}
