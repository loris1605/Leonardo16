using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Tables
{
    public class Person : IStandardTable
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string SurName { get; set; } = string.Empty;
        public int Natoil { get; set; }
        public string UniqueParam {  get; set; } = string.Empty;

        public List<Socio> Soci { get; set; } = [];
        public Operatore? Operatore { get; set; }
        public List<Scheda> Schede { get; set; } = [];

        [NotMapped]
        public string Nome
        {
            get => $"{FirstName} {SurName}";
            set
            {
                // Logica opzionale, ad esempio per lo split del nome
                // o semplicemente per aggiornare la UI
            }
        }

    }
}
