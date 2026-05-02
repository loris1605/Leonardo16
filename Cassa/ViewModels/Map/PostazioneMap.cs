using DTO.Entity;
using ReactiveUI;

namespace ViewModels.BindableObjects
{
    public class PostazioneMap : BindableMap
    {
        public PostazioneMap() { }

        public PostazioneMap(PostazioneDTO dto)
        {
            this.Id = dto.Id;
            this.CodiceTipoPostazione = dto.CodiceTipoPostazione;
            this.NomePostazione = dto.NomePostazione;
            this.NomeTipoPostazione = dto.NomeTipoPostazione;
            this.CodiceReparto = dto.CodiceReparto;
            this.NomeSettore = dto.NomeSettore;
            this.EtichettaSettore = dto.EtichettaSettore;
            this.NomeTipoSettore = dto.NomeTipoSettore;
            this.CodiceTipoRientro = dto.CodiceTipoRientro;
            this.NomeTipoRientro = dto.NomeTipoSettore;
            this.HasPermesso = dto.HasPermesso;
        }

        public PostazioneDTO ToDto()
        {
            return new PostazioneDTO
            {
                Id = this.Id,
                CodiceTipoPostazione = this.CodiceTipoPostazione,
                NomePostazione = this.NomePostazione,
                NomeTipoPostazione = this.NomeTipoPostazione,
                CodiceReparto = this.CodiceReparto,
                NomeSettore = this.NomeSettore,
                EtichettaSettore = this.EtichettaSettore,
                NomeTipoSettore = this.NomeTipoSettore,
                CodiceTipoRientro = this.CodiceTipoRientro,
                NomeTipoRientro = this.NomeTipoSettore,
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

        private int _codicereparto;
        public int CodiceReparto
        {
            get => _codicereparto;
            set => this.RaiseAndSetIfChanged(ref _codicereparto, value);
        }

        private string _nomesettore = string.Empty;
        public string NomeSettore
        {
            get => _nomesettore;
            set => this.RaiseAndSetIfChanged(ref _nomesettore, value);
        }

        private string _etichettasettore = string.Empty;
        public string EtichettaSettore
        {
            get => _etichettasettore;
            set => this.RaiseAndSetIfChanged(ref _etichettasettore, value);
        }

        private string _nometiposettore = string.Empty;
        public string NomeTipoSettore
        {
            get => _nometiposettore;
            set => this.RaiseAndSetIfChanged(ref _nometiposettore, value);
        }

        private int _codicetiporientro;
        public int CodiceTipoRientro
        {
            get => _codicetiporientro;
            set => this.RaiseAndSetIfChanged(ref _codicetiporientro, value);
        }

        private string _nometiporientro = string.Empty;
        public string NomeTipoRientro
        {
            get => _nometiporientro;
            set => this.RaiseAndSetIfChanged(ref _nometiporientro, value);
        }

        private bool _haspermesso;
        public bool HasPermesso
        {
            get => _haspermesso;
            set => this.RaiseAndSetIfChanged(ref _haspermesso, value);
        }

        public override string Titolo => $"{NomePostazione} - {NomeTipoPostazione}";

    }
}
