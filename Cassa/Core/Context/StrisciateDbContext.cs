using Microsoft.EntityFrameworkCore;
using Models.Tables;

namespace Models.Context
{
    public interface IStrisciateDbContext
    {
        DbSet<Person> People { get; set; }
        DbSet<Socio> Soci { get; set; }
        DbSet<Strisciata> Strisciate { get; set; }
        DbSet<Tessera> Tessere { get; set; }
    }

    public class StrisciateDbContext : BaseContext, IStrisciateDbContext
    {
        public DbSet<Strisciata> Strisciate { get; set; }
        public DbSet<Person> People { get; set; } = null!;
        public DbSet<Socio> Soci { get; set; } = null!;
        public DbSet<Tessera> Tessere { get; set; } = null!;
    }
}
