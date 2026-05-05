using DTO.Entity;
using ReactiveUI;
using SysNet.Converters;

namespace ViewModels.BindableObjects
{
    public class PersonMap : BindableMap
    {
        public PersonMap() { }

        public PersonMap(PersonDTO dto)
        {
            this.Id = dto.Id;
            this.Nome = dto.Nome;
            this.Cognome = dto.Cognome;
            this.Natoil = dto.Natoil;
            this.CodiceSocio = dto.CodiceSocio;
            this.NumeroSocio = dto.NumeroSocio;
            this.CodiceTessera = dto.CodiceTessera;
            this.NumeroTessera = dto.NumeroTessera;
            this.Scadenza = dto.Scadenza;
            this.CodiceUnivoco = dto.CodiceUnivoco;
        }

        public PersonDTO ToDto()
        {
            return new PersonDTO
            {
                Id = this.Id,
                Nome = this.Nome,
                Cognome = this.Cognome,
                Natoil = this.Natoil,
                CodiceSocio = this.CodiceSocio,
                NumeroSocio = this.NumeroSocio,
                CodiceTessera = this.CodiceTessera,
                NumeroTessera = this.NumeroTessera,
                Scadenza = this.Scadenza,
                CodiceUnivoco = this.CodiceUnivoco
            };

            
        }

        private string _cognome;
        public string Cognome
        {
            get => _cognome;
            set => this.RaiseAndSetIfChanged(ref _cognome, value);
        }

        private string nome = string.Empty;
        public override string Nome
        {
            get => nome;
            set => this.RaiseAndSetIfChanged(ref nome, value);

        }

        private int _natoil;
        public int Natoil
        {
            get => _natoil;
            set => this.RaiseAndSetIfChanged(ref _natoil, value);

        }

        private int _codicesocio;
        public int CodiceSocio
        {
            get => _codicesocio;
            set => this.RaiseAndSetIfChanged(ref _codicesocio, value);

        }

        private string numerosocio = string.Empty;
        public string NumeroSocio
        {
            get => numerosocio;
            set => this.RaiseAndSetIfChanged(ref numerosocio, value);

        }

        private int _codicetessera;
        public int CodiceTessera
        {
            get => _codicetessera;
            set => this.RaiseAndSetIfChanged(ref _codicetessera, value);

        }

        private string numerotessera = string.Empty;
        public string NumeroTessera
        {
            get => numerotessera;
            set => this.RaiseAndSetIfChanged(ref numerotessera, value);

        }

        private int _scadenza;
        public int Scadenza
        {
            get => _scadenza;
            set => this.RaiseAndSetIfChanged(ref _scadenza, value);

        }

        private string _codiceunivoco = string.Empty;
        public string CodiceUnivoco
        {
            get => _codiceunivoco;
            set => this.RaiseAndSetIfChanged(ref _codiceunivoco, value);

        }



        // 2. Aggiungi un controllo di sicurezza sulle date (se l'int è 0, ToShortDateString crasha)
        public override string Titolo => $"{Nome} {Cognome} ({NatoilDate.ToShortDateString()})";

        public DateTime NatoilDate => Natoil.DateIntToDate();
        public DateTime ScadenzaDate => Scadenza.DateIntToDate();

        public bool IsMaggiorenne => Natoil.IsLegalAge();

        
    }
}
