namespace Models.Tables
{
    public class TipoFidelity : IStandardTable
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public int DurataGG { get; set; }
        public int TipoFidelityInputId { get; set; }
        public int TipoFidelityOutputId { get; set; }

        public TipoFidelityInput? TipoFidelityInput { get; set; }
        public TipoFidelityOutput? TipoFidelityOutput { get; set; } 

        public List<Fidelity> Fidelities { get; set; } = [];
    }
}
