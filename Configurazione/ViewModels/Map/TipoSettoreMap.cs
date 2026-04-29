using DTO.Entity;
using Models.Tables;
using ReactiveUI;
using System.Linq.Expressions;

namespace ViewModels.BindableObjects
{
    public class TipoSettoreMap : BindableMap
    {
        public TipoSettoreMap() { }

        private string _nometiposettore = string.Empty;
        public override string Nome
        {
            get => _nometiposettore;
            set => this.RaiseAndSetIfChanged(ref _nometiposettore, value);
        }
         public TipoSettoreMap(DTO.Entity.TipoSettoreDTO dto)
        {
            this.Id = dto.Id;
            this.Nome = dto.Nome;
        }

        public TipoSettoreDTO ToDto()
        {
            return new DTO.Entity.TipoSettoreDTO
            {
                Id = this.Id,
                Nome = this.Nome
            };
        }

        
    }
}
