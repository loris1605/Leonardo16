using Microsoft.EntityFrameworkCore;
using Models.Tables;

namespace Models.Context
{
    public interface ILoginDbContext
    {
        DbSet<Giornata> Giornate { get; set; }
        DbSet<Listino> Listini { get; set; }
        DbSet<Operatore> Operatori { get; set; }
        DbSet<Person> People { get; set; }
        DbSet<Permesso> Permessi { get; set; }
        DbSet<Postazione> Postazioni { get; set; }
        DbSet<Reparto> Reparti { get; set; }
        DbSet<Settore> Settori { get; set; }
        DbSet<Tariffa> Tariffe { get; set; }
        DbSet<TipoPostazione> TipiPostazione { get; set; }
        DbSet<TipoRientro> TipiRientro { get; set; }
        DbSet<TipoSettore> TipiSettore { get; set; }
    }

    public class LoginDbContext : BaseContext, ILoginDbContext
    {
        public DbSet<Person> People { get; set; } = null!;
        public DbSet<Operatore> Operatori { get; set; } = null!;
        public DbSet<Postazione> Postazioni { get; set; } = null!;
        public DbSet<Permesso> Permessi { get; set; } = null!;
        public DbSet<Reparto> Reparti { get; set; } = null!;
        public DbSet<Listino> Listini { get; set; } = null!;
        public DbSet<TipoPostazione> TipiPostazione { get; set; } = null!;
        public DbSet<TipoRientro> TipiRientro { get; set; } = null!;
        public DbSet<TipoSettore> TipiSettore { get; set; } = null!;
        public DbSet<Giornata> Giornate { get; set; } = null!;
        public DbSet<Settore> Settori { get; set; } = null!;
        public DbSet<Tariffa> Tariffe { get; set; } = null!;
    }
}
