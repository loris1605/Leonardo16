namespace Models.Entity.Global
{
    public class TurnoXC
    {
        public int CODICETURNO { get; set; }

        public DateTime DATAINIZIO { get; set; }

        public DateTime DATAFINE { get; set; }

        public bool APERTO { get; set; }

        public int IDGIORNATA { get; set; }

        public int IDOPERATORE { get; set; }

        public decimal CASSAINIZIALE { get; set; }

        public decimal CASSAFINALE { get; set; }

        public decimal DIFFERENZA { get; set; }


    }
}
