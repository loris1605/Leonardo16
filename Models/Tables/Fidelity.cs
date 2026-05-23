using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Tables
{
    public class Fidelity : IStandardTable
    {
        public int Id { get; set; }
        public int TipoFidelityId { get; set; }
        public int PersonId { get; set; }

        public TipoFidelity? TipoFidelity { get; set; }
        public Person? Person { get; set; }

        public List<FidelityEntry> FidelityEntries { get; set; } = [];


        [NotMapped]
        public string Nome
        {
            // Restituisce il nome della postazione se caricata, altrimenti una stringa vuota o ID
            get => "";
            set { }
        }
    }
}
