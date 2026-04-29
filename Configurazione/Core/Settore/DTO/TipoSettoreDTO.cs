using Models.Interfaces;
using Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Entity
{
    public class TipoSettoreDTO : BaseDTO, IMap, IMappable<TipoSettore>
    {
        public TipoSettoreDTO() { }
        public TipoSettoreDTO(TipoSettore table)
        {
            this.Id = table.Id;
            this.Nome = table.Nome;
        }
        public TipoSettore ToTable()
        {
            return new TipoSettore
            {
                Id = this.Id,
                Nome = this.Nome
            };
        }
        public void UpdateTable(TipoSettore existing)
        {
            if (existing == null) return;
            existing.Nome = this.Nome;
        }

        public static Expression<Func<TipoSettore, TipoSettoreDTO>> ToDto => t => new TipoSettoreDTO
        {
            Id = t.Id,
            Nome = t.Nome
        };
    }
}
