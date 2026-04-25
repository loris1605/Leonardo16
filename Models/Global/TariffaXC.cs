namespace Models.Entity.Global
{
    public class TariffaXC
    {
        public int CODICETARIFFA { get; set; }

        private string _mydesc = string.Empty;
        public string DESCTARIFFA
        {
            get => _mydesc is null ? string.Empty : _mydesc;
            set => _mydesc = value is null ? string.Empty : value;
        }

        public decimal PRICETARIFFA { get; set; }


    }
}
