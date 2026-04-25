namespace Models.Tables
{
    public class Giornata
    {
        public int Id { get; set; }
        public DateTime DataInizio {  get; set; }
        public DateTime DataFine { get; set; }
        public bool Aperta { get; set; }
    }
}
