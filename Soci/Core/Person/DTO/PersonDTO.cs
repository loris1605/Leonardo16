using Models.Interfaces;
using Models.Tables;
using SysNet.Converters;

namespace DTO.Entity
{
    public class PersonDTO : BaseDTO, IMap, IMappable<Person>
    {
        public PersonDTO() { }  

        public PersonDTO(Person table)
        {
            this.Id = table.Id;
            this.Cognome = table.SurName;
            this.Nome = table.FirstName;
            this.Natoil = table.Natoil;
            this.CodiceUnivoco = table.UniqueParam;

            var primoSocio = table.Soci.FirstOrDefault();

            this.CodiceSocio = primoSocio?.Id ?? 0;
            this.NumeroSocio = primoSocio?.NumeroSocio ?? "0";

            var primaTessera = primoSocio?.Tessere?.FirstOrDefault();

            this.CodiceTessera = primaTessera?.Id ?? 0;
            this.NumeroTessera = primaTessera?.NumeroTessera ?? string.Empty;
            this.Scadenza = primaTessera?.Scadenza ?? 0;
        }

        public Person ToTable()
        {
            return new Person
            {
                Id = this.Id,
                SurName = this.Cognome,
                FirstName = this.Nome,
                Natoil = this.Natoil,
                UniqueParam = this.CodiceUnivoco
            };
            
        }

        public void UpdateTable(Person existing)
        {
            if (existing == null) return;
            // Aggiorniamo solo i campi che possono cambiare
            existing.SurName = this.Cognome;
            existing.FirstName = this.Nome;
            existing.Natoil = this.Natoil;
            existing.UniqueParam = this.CodiceUnivoco;
            // Non tocchiamo l'ID!
        }

        public string Cognome { get; set; } = string.Empty;
        public int Natoil { get; set; }
        
        //public DateTime? NatoilDate { get; set; }
        public int CodiceSocio { get; set; }
        public string NumeroSocio { get; set; } = string.Empty;
        public int CodiceTessera { get; set; }
        public string NumeroTessera { get; set; } = string.Empty;
        public int Scadenza { get; set; }
        //public DateTime? ScadenzaDate { get; set; } = DateTime.Now;

        public string CodiceUnivoco { get; set; } = string.Empty;

        // 2. Aggiungi un controllo di sicurezza sulle date (se l'int è 0, ToShortDateString crasha)
        public override string Titolo => $"{Nome} {Cognome} ({Natoil.DateIntToDate().ToShortDateString()})";

        
    }

    


}
