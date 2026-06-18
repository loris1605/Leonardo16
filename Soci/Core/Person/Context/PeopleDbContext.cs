

using Microsoft.EntityFrameworkCore;
using Models.Context;
using Models.Tables;
using Soci.Interfaces.Core;

namespace Soci.Models.Context
{
    

    public class PeopleDbContext : BaseContext, IPeopleDbContext
    {
        public DbSet<Person> People { get; set; } = null!;
        public DbSet<Socio> Soci { get; set; } = null!;
        public DbSet<Tessera> Tessere { get; set; } = null!;
    }
}
