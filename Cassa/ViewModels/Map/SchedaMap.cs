using DTO.Entity;
using ReactiveUI;

namespace ViewModels.BindableObjects
{
    public class SchedaMap : BindableMap
    {
        public SchedaMap() { }

        public SchedaMap(SchedaDTO dto)
        {
            this.Id = dto.Id;
            this.Posizione = dto.Posizione;
            this.NumeroTessera = dto.NumeroTessera;
            this.CodicePerson = dto.CodicePerson;
            this.Cognome = dto.Cognome;
            this.Natoil = dto.Natoil;
            this.CheckinTime = dto.CheckinTime;
            this.CheckoutTime = dto.CheckoutTime;
            this.Grb1 = dto.Grb1;
            this.Grb2 = dto.Grb2;
            this.Grb3 = dto.Grb3;
            this.Grb4 = dto.Grb4;
            this.Consumazione = dto.Consumazione;
            this.Blocco = dto.Blocco;
            this.Note = dto.Note;

        }


        public SchedaDTO ToDto()
        {
            return new SchedaDTO
            {
                Id = this.Id,
                Posizione = this.Posizione,
                NumeroTessera = this.NumeroTessera,
                CodicePerson = this.CodicePerson,
                Cognome = this.Cognome,
                Natoil = this.Natoil,
                CheckinTime = this.CheckinTime,
                CheckoutTime = this.CheckoutTime,
                Grb1 = this.Grb1,
                Grb2 = this.Grb2,
                Grb3 = this.Grb3,
                Grb4 = this.Grb4,
                Consumazione = this.Consumazione,
                Blocco = this.Blocco,
                Note = this.Note
            };
        }

        private string _posizione = string.Empty;
        public string Posizione
        {
            get => _posizione;
            set => this.RaiseAndSetIfChanged(ref _posizione, value);
        }

        private string _numeroTessera = string.Empty;
        public string NumeroTessera
        {
            get => _numeroTessera;
            set => this.RaiseAndSetIfChanged(ref _numeroTessera, value);
        }

        private int _codicePerson;
        public int CodicePerson
        {
            get => _codicePerson;
            set => this.RaiseAndSetIfChanged(ref _codicePerson, value);
        }

        private string _cognome = string.Empty;
        public string Cognome
        {
            get => _cognome;
            set => this.RaiseAndSetIfChanged(ref _cognome, value);
        }

        private int _natoil;
        public int Natoil
        {
            get => _natoil;
            set => this.RaiseAndSetIfChanged(ref _natoil, value);
        }

        private DateTime _checkinTime = DateTime.Now;
        public DateTime CheckinTime
        {
            get => _checkinTime;
            set => this.RaiseAndSetIfChanged(ref _checkinTime, value);
        }

        private DateTime _checkoutTime = DateTime.MaxValue;
        public DateTime CheckoutTime
        {
            get => _checkoutTime;
            set => this.RaiseAndSetIfChanged(ref _checkoutTime, value);
        }

        private int _grb1;
        public int Grb1
        {
            get => _grb1;
            set => this.RaiseAndSetIfChanged(ref _grb1, value);
        }

        private int _grb2;
        public int Grb2
        {
            get => _grb2;
            set => this.RaiseAndSetIfChanged(ref _grb2, value);
        }

        private int _grb3;
        public int Grb3
        {
            get => _grb3;
            set => this.RaiseAndSetIfChanged(ref _grb3, value);
        }

        private int _grb4;
        public int Grb4
        {
            get => _grb4;
            set => this.RaiseAndSetIfChanged(ref _grb4, value);
        }

        private decimal _consumazione;
        public decimal Consumazione
        {
            get => _consumazione;
            set => this.RaiseAndSetIfChanged(ref _consumazione, value);
        }

        private bool _blocco;
        public bool Blocco
        {
            get => _blocco;
            set => this.RaiseAndSetIfChanged(ref _blocco, value);
        }

        private string _note = string.Empty;
        public string Note
        {
            get => _note;
            set => this.RaiseAndSetIfChanged(ref _note, value);
        }

    }

}
