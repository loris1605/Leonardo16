using Microsoft.EntityFrameworkCore;
using Models.Tables;

namespace Models.Context
{
    public interface ISettoreDbContext
    {
        DbSet<Listino> Listini { get; set; }
        DbSet<Reparto> Reparti { get; set; }
        DbSet<Settore> Settori { get; set; }
        DbSet<Tariffa> Tariffe { get; set; }
        DbSet<TipoSettore> TipiSettore { get; set; }
    }

    public class SettoreDbContext : BaseContext, ISettoreDbContext
    {
        public DbSet<Settore> Settori { get; set; } = null!;
        public DbSet<Listino> Listini { get; set; } = null!;
        public DbSet<TipoSettore> TipiSettore { get; set; } = null!;
        public DbSet<Tariffa> Tariffe { get; set; } = null!;
        public DbSet<Reparto> Reparti { get; set; } = null!;
    }
}
