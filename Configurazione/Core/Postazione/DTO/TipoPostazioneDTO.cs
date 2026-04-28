using Models.Entity;
using Models.Interfaces;
using Models.Tables;
using System.Linq.Expressions;

namespace DTO.Entity
{
    public class TipoPostazioneDTO : BaseDTO, IMap, IMappable<TipoPostazione>
    {
        public TipoPostazioneDTO() { }

        public TipoPostazioneDTO(TipoPostazione table)
        {
            this.Id = table.Id;
            this.Nome = table.Nome;
        }

        public TipoPostazione ToTable()
        {
            return new TipoPostazione
            {
                Id = this.Id,
                Nome = this.Nome
            };
        }

        public void UpdateTable(TipoPostazione existing)
        {
            if (existing == null) return;
            existing.Nome = this.Nome;
        }

        public static Expression<Func<TipoPostazione, TipoPostazioneDTO>> ToDto => t => new TipoPostazioneDTO
        {
            Id = t.Id,
            Nome = t.Nome
        };
    }
}
