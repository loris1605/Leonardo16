namespace Models.Tables
{
    public class TipoFidelityInput : IStandardTable
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        

        public List<TipoFidelity> TipiFidelity { get; set; } = new List<TipoFidelity>();
    }
}
