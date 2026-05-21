using Models.Interfaces;

namespace DTO.Entity
{
    public class EntraIngressiDTO : BaseDTO, IMap
    {
        public string NomeTariffa { get; set; } = string.Empty;
        public string EtichettaTariffa { get; set; } = string.Empty;
        public decimal PrezzoTariffa { get; set; } = decimal.Zero;
        public bool IsFreeDrink { get; set; }
        
    }
}
