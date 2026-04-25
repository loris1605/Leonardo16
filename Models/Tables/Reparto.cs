namespace Models.Tables
{
    public class Reparto
    {
        public int Id { get; set; }
        public int SettoreId { get; set; }
        public int PostazioneId { get; set; }

        public Settore? Settore { get; set; }
        public Postazione? Postazione { get; set; }
    }
}
