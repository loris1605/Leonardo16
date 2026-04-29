using DTO.Entity;
using ReactiveUI;

namespace ViewModels.BindableObjects
{
    public class TipoPostazioneMap : BindableMap
    {
        public TipoPostazioneMap() { }

        private string _nomepostazione = string.Empty;
        public override string Nome
        {
            get => _nomepostazione;
            set => this.RaiseAndSetIfChanged(ref _nomepostazione, value);
        }

        public TipoPostazioneMap(TipoPostazioneDTO dto)
        {
            this.Id = dto.Id;
            this.Nome = dto.Nome;
            
        }

        public TipoPostazioneDTO ToDto()
        {
            return new TipoPostazioneDTO
            {
                Id = this.Id,
                Nome = this.Nome
               
            };
        }
    }
}
