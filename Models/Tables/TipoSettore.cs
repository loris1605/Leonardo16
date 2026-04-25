namespace Models.Tables
{
    public class TipoSettore
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;

        public List<Settore> Settori { get; set; } = [];
    }
}
