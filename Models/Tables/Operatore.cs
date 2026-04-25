namespace Models.Tables
{
    public class Operatore : IStandardTable
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool Abilitato { get; set; }
        public int Pass {  get; set; }

        public int PersonId { get; set; }
        public Person? Person { get; set; }

        public List<Permesso> Permessi { get; set; } = [];

    }
}
