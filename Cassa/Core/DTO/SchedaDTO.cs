using Models.Interfaces;
using Models.Tables;

namespace DTO.Entity
{
    public class SchedaDTO : BaseDTO, IMap
    {
        public string Posizione { get; set; } = string.Empty;
        public string NumeroTessera { get; set; } = string.Empty;
        public int CodicePerson { get; set; }
        public string Cognome { get; set; } = string.Empty;
        public int Natoil { get; set; }
        public DateTime CheckinTime { get; set; } = DateTime.Now;
        public DateTime CheckoutTime { get; set; } = DateTime.MaxValue;
        public int Grb1 { get; set; }
        public int Grb2 { get; set; }
        public int Grb3 { get; set; }
        public int Grb4 { get; set; }
        public decimal Consumazione { get; set; }
        public bool Blocco { get; set; }
        public string Note { get; set; } = string.Empty;

        public SchedaDTO() { }

        public SchedaDTO(Scheda table)
        {

            this.Id = table.Id;
            this.Posizione = table.Posizione;
            this.NumeroTessera = table.NumeroTessera;
            this.CodicePerson = table.PersonId;
            this.Cognome = table.Cognome;
            this.Natoil = table.Natoil;
            this.CheckinTime = table.CheckinTime;
            this.CheckoutTime = table.CheckoutTime;
            this.Grb1 = table.Grb1;
            this.Grb2 = table.Grb2;
            this.Grb3 = table.Grb3;
            this.Grb4 = table.Grb4;
            this.Consumazione = table.Consumazione;
            this.Blocco = table.Blocco;
            this.Note = table.Note;
        }

        public Scheda ToTable()
        {
            return new Scheda
            {
                Id = this.Id,
                Posizione = this.Posizione,
                NumeroTessera = this.NumeroTessera,
                PersonId = this.CodicePerson,
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

        public void UpdateTable(Scheda existing)
        {
            if (existing == null) return;

            existing.Posizione = this.Posizione;
            existing.NumeroTessera = this.NumeroTessera;
            existing.PersonId = this.CodicePerson;
            existing.Cognome = this.Cognome;
            existing.Natoil = this.Natoil;
            existing.CheckinTime = this.CheckinTime;
            existing.CheckoutTime = this.CheckoutTime;
            existing.Grb1 = this.Grb1;
            existing.Grb2 = this.Grb2;
            existing.Grb3 = this.Grb3;
            existing.Grb4 = this.Grb4;
            existing.Consumazione = this.Consumazione;
            existing.Blocco = this.Blocco;
            existing.Note = this.Note;

        }
    }
}
