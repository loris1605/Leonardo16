using DTO.Entity;
using Models.Tables;
using ReactiveUI;

namespace ViewModels.BindableObjects
{
    public class OperatoreMap : BindableMap
    {
        public OperatoreMap() { }

        public OperatoreMap(OperatoreDTO dto)
        {
            this.Id = dto.Id;
            this.NomeOperatore = dto.NomeOperatore;
            this.Password = dto.Password;
            this.Badge = dto.Badge;
            this.Abilitato = dto.Abilitato;
            this.CodicePerson = dto.CodicePerson;
            this.NomePostazione = dto.NomePostazione;
            this.TipoPostazione = dto.TipoPostazione;
            this.CodicePermesso = dto.CodicePermesso;
        }

        public OperatoreDTO ToDto()
        {
            return new OperatoreDTO
            {
                Id = this.Id,
                NomeOperatore = this.NomeOperatore,
                Password = this.Password,
                Badge = this.Badge,
                Abilitato = this.Abilitato,
                CodicePerson = this.CodicePerson,
                CodicePermesso = this.CodicePermesso
            };
         }

        

        private string _nomeoperatore = string.Empty;
        public string NomeOperatore
        {
            get => _nomeoperatore;
            set => this.RaiseAndSetIfChanged(ref _nomeoperatore, value);
        }

        private string _password = string.Empty;
        public string Password
        {
            get => _password;
            set => this.RaiseAndSetIfChanged(ref _password, value);
        }

        private bool _abilitato = true;
        public bool Abilitato
        {
            get => _abilitato;
            set => this.RaiseAndSetIfChanged(ref _abilitato, value);
        }

        private int _badge = 0;
        public int Badge
        {
            get => _badge;
            set => this.RaiseAndSetIfChanged(ref _badge, value);
        }

        private int _codicepermesso = 0;
        public int CodicePermesso
        {
            get => _codicepermesso;
            set => this.RaiseAndSetIfChanged(ref _codicepermesso, value);
        }

        private string _nomepostazione = string.Empty;
        public string NomePostazione
        {
            get => _nomepostazione;
            set => this.RaiseAndSetIfChanged(ref _nomepostazione, value);
        }

        private string _tipopostazione = string.Empty;
        public string TipoPostazione
        {
            get => _tipopostazione;
            set => this.RaiseAndSetIfChanged(ref _tipopostazione, value);
        }

        private int _codiceperson = 0;
        public int CodicePerson
        {
            get => _codiceperson;
            set => this.RaiseAndSetIfChanged(ref _codiceperson, value);
        }

        public override string Titolo => $"{NomeOperatore} - {(Abilitato ? "Abilitato" : "Non abilitato")}";
    }
}
