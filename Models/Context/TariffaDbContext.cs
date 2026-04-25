using Microsoft.EntityFrameworkCore;
using Models.Tables;

namespace Models.Context
{
    public class TariffaDbContext : BaseContext
    {
        public DbSet<Tariffa> Tariffe { get; set; } = null!;
        public DbSet<Listino> Listini { get; set; } = null!;
    }
}
