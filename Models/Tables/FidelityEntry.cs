using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Tables
{
    public class FidelityEntry : IStandardTable
    {
        public int Id { get; set; }
        public int FidelityId { get; set; }
        public bool IsChecked { get; set; }
        public int DataIngresso { get; set; }

        public Fidelity? Fidelity { get; set; }


        [NotMapped]
        public string Nome
        {
            // Restituisce il nome della postazione se caricata, altrimenti una stringa vuota o ID
            get => "";
            set { }
        }

    }
}
