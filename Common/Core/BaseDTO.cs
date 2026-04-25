namespace DTO.Entity
{
    public class BaseDTO 
    {
        public int Id { get; set; }
        public virtual string? Titolo { get; set; } = string.Empty;
        public virtual string Nome { get; set; } = String.Empty;
        public override string ToString() => Nome ?? string.Empty;
    }

    public interface IDTO
    {
        int Id { get; set; }
        string Nome { get; set; }
    }
}
