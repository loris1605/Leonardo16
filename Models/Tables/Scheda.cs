using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Tables
{
    public class Scheda : IStandardTable
    {
        public int Id { get; set; }

        public string Posizione { get; set; } = string.Empty;
        public string NumeroTessera { get; set; } = string.Empty;
        public int PersonId { get; set; }
        public string Cognome { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
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

        public Person? Person { get; set; }


    }
}
