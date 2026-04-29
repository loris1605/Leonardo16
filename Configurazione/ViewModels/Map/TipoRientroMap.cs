using DTO.Entity;
using ReactiveUI;

namespace ViewModels.BindableObjects
{
    public class TipoRientroMap : BindableMap
    {
        public TipoRientroMap() { }

        public TipoRientroMap(TipoRientroDTO dto)
        {
            this.Id = dto.Id;
            this.Nome = dto.Nome;
            this.DurataOre = dto.DurataOre;

        }

        public TipoRientroDTO ToDto()
        {
            return new TipoRientroDTO
            {
                Id = this.Id,
                Nome = this.Nome,
                DurataOre = this.DurataOre

            };
        }

        private string _nomepostazione = string.Empty;
        public override string Nome
        {
            get => _nomepostazione;
            set => this.RaiseAndSetIfChanged(ref _nomepostazione, value);
        }

        private int _mydurataore;
        public int DurataOre
        {
            get => _mydurataore;
            set => this.RaiseAndSetIfChanged(ref _mydurataore, value);
        }
    }
}
