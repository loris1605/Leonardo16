using Microsoft.EntityFrameworkCore;
using Models.Tables;

namespace Models.Context
{
    public class PostazioneDbContext : BaseContext
    {
        //public DbSet<Operatore> Operatori { get; set; } = null!;
        public DbSet<Permesso> Permessi { get; set; } = null!;
        public DbSet<Postazione> Postazioni { get; set; } = null!;
        public DbSet<TipoPostazione> TipiPostazione { get; set; } = null!;
        public DbSet<Reparto> Reparti { get; set; } = null!;
        public DbSet<Settore> Settori { get; set; } = null!;
        public DbSet<TipoRientro> TipiRientro { get; set; } = null!;
        public DbSet<TipoSettore> TipiSettore { get; set; } = null!;
    }
}
