using Microsoft.EntityFrameworkCore;
using Models.Tables;

namespace Models.Context
{
    public interface IEntraSocioDbContext
    {
        DbSet<Person> People { get; set; }
        DbSet<Socio> Soci { get; set; }
        DbSet<Tessera> Tessere { get; set; }
        DbSet<Tariffa> Tariffe { get; set; }
        DbSet<Settore> Settori { get; set; }
        DbSet<Postazione> Postazioni { get; set; }
        DbSet<Listino> Listini { get; set; }
        DbSet<Reparto> Reparti { get; set; }

    }

    public class EntraSocioDbContext : BaseContext, IEntraSocioDbContext
    {
        public DbSet<Person> People { get; set; } = null!;
        public DbSet<Socio> Soci { get; set; } = null!;
        public DbSet<Tessera> Tessere { get; set; } = null!;
        public DbSet<Tariffa> Tariffe { get; set; } = null!;
        public DbSet<Settore> Settori { get; set; } = null!;
        public DbSet<Postazione> Postazioni { get; set; } = null!;
        public DbSet<Listino> Listini { get; set; } = null!;
        public DbSet<Reparto> Reparti { get; set; } = null!;
    }
}
