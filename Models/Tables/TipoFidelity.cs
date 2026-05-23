namespace Models.Tables
{
    public class TipoFidelity : IStandardTable
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public int EntryBeforeFree { get; set; }
        public int DurataGG { get; set; }

        public List<Fidelity> Fidelities { get; set; } = [];
    }
}
