using Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Entity
{
    public class EntraSocioDTO : BaseDTO, IMap
    {
        public string NumeroTessera { get; set; } = string.Empty;
        public int CodicePerson { get; set; }
        public string Cognome { get; set; } = string.Empty;
        public int Natoil { get; set; }
        public bool Blocco { get; set; }
        public string NumeroSocio { get; set; }
        public int ScadenzaTessera { get; set; }

    }
}
