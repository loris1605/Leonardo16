using Microsoft.EntityFrameworkCore;
using Models.Tables;

namespace Models.Context
{
    public interface IMenuDbContext
    {
        DbSet<Giornata> Giornate { get; set; }
        DbSet<Permesso> Permessi { get; set; }
        DbSet<Postazione> Postazioni { get; set; }
        DbSet<TipoPostazione> TipiPostazione { get; set; }
    }

    public class MenuDbContext : BaseContext, IMenuDbContext
    {
        public DbSet<Giornata> Giornate { get; set; } = null!;
        public DbSet<TipoPostazione> TipiPostazione { get; set; } = null!;
        public DbSet<Postazione> Postazioni { get; set; } = null!;
        public DbSet<Permesso> Permessi { get; set; } = null!;

    }
}
