using DTO.Entity;
using ReactiveUI;

namespace ViewModels.BindableObjects
{
    public class PostazioneElencoMap : BindableMap
    {
        public PostazioneElencoMap() { }

        public PostazioneElencoMap(PostazioneElencoDTO dto)
        {
            this.Id = dto.Id;
            this.CodiceTipoPostazione = dto.CodiceTipoPostazione;
            this.NomePostazione = dto.NomePostazione;
            this.NomeTipoPostazione = dto.NomeTipoPostazione;
            this.HasPermesso = dto.HasPermesso;
        }

        public PostazioneElencoDTO ToDto()
        {
            return new PostazioneElencoDTO
            {
                Id = this.Id,
                CodiceTipoPostazione = this.CodiceTipoPostazione,
                NomePostazione = this.NomePostazione,
                NomeTipoPostazione = this.NomeTipoPostazione,
                HasPermesso = this.HasPermesso,
            };
        }


        private int _codicetipopostazione;
        public int CodiceTipoPostazione
        {
            get => _codicetipopostazione;
            set => this.RaiseAndSetIfChanged(ref _codicetipopostazione, value);
        }

        private string _nomepostazione = string.Empty;
        public string NomePostazione
        {
            get => _nomepostazione;
            set => this.RaiseAndSetIfChanged(ref _nomepostazione, value);
        }

        private string _nometipopostazione = string.Empty;
        public string NomeTipoPostazione
        {
            get => _nometipopostazione;
            set => this.RaiseAndSetIfChanged(ref _nometipopostazione, value);
        }

        private bool _haspermesso;
        public bool HasPermesso
        {
            get => _haspermesso;
            set => this.RaiseAndSetIfChanged(ref _haspermesso, value);
        }
    }
}
