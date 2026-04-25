namespace Models.Tables
{
    public class TipoRientro
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public int DurataOre { get; set; }

        public List<Postazione> Postazioni { get; set; } = [];

    }
}
