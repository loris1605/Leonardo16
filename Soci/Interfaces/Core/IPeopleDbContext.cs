using Microsoft.EntityFrameworkCore;
using Models.Tables;

namespace Soci.Interfaces.Core
{
    public interface IPeopleDbContext
    {
        DbSet<Person> People { get; set; }
        DbSet<Socio> Soci { get; set; }
        DbSet<Tessera> Tessere { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
