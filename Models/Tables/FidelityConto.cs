using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Tables
{
    public class FidelityConto : IStandardTable
    {
        public int Id { get; set; }
        public int FidelityId { get; set; }
        public int Value { get; set; }
        public int DataOra { get; set; }

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
