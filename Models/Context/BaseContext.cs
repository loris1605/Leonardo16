using Microsoft.EntityFrameworkCore;
using Models.Tables;
using SysNet;

namespace Models.Context
{
    public class BaseContext : DbContext
    {
        //public BaseContext(DbContextOptions<AppDbContext> options) : base(options)
        //{

        //}

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // Aggiunto "Encrypt=False" per evitare problemi di certificati SSL su LocalDB
            options.UseSqlServer(conn);
        }

        readonly string conn = Connection.CurrentConnectionString;

        
    }
}
