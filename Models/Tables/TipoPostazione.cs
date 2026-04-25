namespace Models.Tables
{
    public class TipoPostazione
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;

        public List<Postazione> Postazioni { get; set; } = [];

    }
}
