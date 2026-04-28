using Models.Interfaces;
using Models.Tables;
using System.Linq.Expressions;

namespace DTO.Entity
{
    public class TipoRientroDTO : BaseDTO, IMap, IMappable<TipoRientro>
    {
        public TipoRientroDTO() { }

        public TipoRientroDTO(TipoRientro table)
        {
            this.Id = table.Id;
            this.Nome = table.Nome;
            this.DurataOre = table.DurataOre;
        }

        public int DurataOre { get; set; }

        public TipoRientro ToTable()
        {
            return new TipoRientro
            {
                Id = this.Id,
                Nome = this.Nome,
                DurataOre = this.DurataOre
            };
        }

        public void UpdateTable(TipoRientro existing)
        {
            if (existing == null) return;
            existing.Nome = this.Nome;
            existing.DurataOre = this.DurataOre;
        }

        public static Expression<Func<TipoRientro, TipoRientroDTO>> ToDto => t => new TipoRientroDTO
        {
            Id = t.Id,
            Nome = t.Nome,
            DurataOre = t.DurataOre
        };
    }
}
