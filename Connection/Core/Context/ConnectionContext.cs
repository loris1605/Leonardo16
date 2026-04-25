using Microsoft.EntityFrameworkCore;
using Models.Context;
using Models.Tables;

namespace Connection.Core.Context
{
    public class ConnectionContext : BaseContext
    {
        public DbSet<DbSettings> Settings { get; set; }
    }
}
