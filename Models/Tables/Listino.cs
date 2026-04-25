using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Tables
{
    public class Listino
    {
        public int Id { get; set; }
        public int SettoreId { get; set; }
        public int TariffaId { get; set; }

        public Settore? Settore { get; set; }
        public Tariffa? Tariffa { get; set; }
    }
}
