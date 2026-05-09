using Microsoft.EntityFrameworkCore;
using Models.Context;
using Models.Repository;
using Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Repository
{
    public class StrisciataRepository : BaseRepository<StrisciateDbContext, Strisciata>
    {
        private readonly IStrisciateDbContext _ctx;

        public StrisciataRepository(IStrisciateDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<List<Strisciata>> GetStrisciate(CancellationToken ctk = default)
        {
            var data = await _ctx.Strisciate.OrderBy(x => x.Id).ToListAsync(ctk);
            return data;
        }

        public async Task DevelopStrisciate(CancellationToken ctk = default)
        {
            var data = await GetStrisciate(ctk);
            if (data.Count == 0) return;

            foreach (Strisciata item in data)
            {
                // 1° caso - Verifichiamo se la UniqueIndentifier esiste già

            }

            await Task.CompletedTask;
        }
    }
}
