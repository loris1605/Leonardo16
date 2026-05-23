using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Tables
{
    public class FidelityEntry : IStandardTable
    {
        public int Id { get; set; }
        public int FidelityId { get; set; }
        public bool IsChecked { get; set; }
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
