using Microsoft.EntityFrameworkCore;
using Models.Tables;

namespace Models.Context
{
    public class PeopleDbContext : BaseContext
    {
        public DbSet<Person> People { get; set; } = null!;
        public DbSet<Socio> Soci { get; set; } = null!;
        public DbSet<Tessera> Tessere { get; set; } = null!;
    }
}
