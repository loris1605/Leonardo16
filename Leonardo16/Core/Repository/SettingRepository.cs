using Leonardo16.Core.Context;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Leonardo16.Core.Repository
{
    public interface ISettingRepository
    {
        Task<bool> CheckAppVersion(int appVersion);
    }

    public class SettingRepository : ISettingRepository
    {
        private readonly ISettingDbContext _dbx;

        public SettingRepository(ISettingDbContext dbx)
        {
            _dbx = dbx;
            Debug.WriteLine($"***** [GC] {this.GetType().Name} {this.GetHashCode()} CARICATO *****");
        }

#if DEBUG
        ~SettingRepository()
        {
            // Questo apparirà nella finestra "Output" di Visual Studio
            Debug.WriteLine($"***** [GC] {this.GetType().Name} {this.GetHashCode()} DISTRUTTO *****");
        }
#endif

        public async Task<bool> CheckAppVersion(int appVersion)
        {
            return await _dbx.Settings
                        .AsNoTracking()
                        .AnyAsync(s => s.Version == appVersion);
        }
    }
}
