using Microsoft.EntityFrameworkCore;
using Models.Tables;

namespace Models.Context
{
    public interface ICassaPostazioneDbContext
    {
        DbSet<Postazione> Postazioni { get; set; }
    }

    public class CassaPostazioneDbContext : BaseContext, ICassaPostazioneDbContext
    {
        public DbSet<Postazione> Postazioni { get; set; } = null;
    }
}
