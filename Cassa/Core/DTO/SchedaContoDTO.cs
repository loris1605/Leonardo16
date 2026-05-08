using Models.Interfaces;
using Models.Tables;

namespace DTO.Entity
{
    public class SchedaContoDTO : BaseDTO, IMap
    {
        public int CodiceScheda { get; set; }
        public string DescSettore { get; set; } = string.Empty;
        public string DescPostazione {  get; set; } = string.Empty;
        public string VoiceDesc {  get; set; } = string.Empty;
        public decimal VoicePrice { get; set; }
        public bool Pagato { get; set; }
        public string Note { get; set; } = string.Empty;
        public DateTime DataOra { get; set; }

        public SchedaContoDTO() { }

        public SchedaContoDTO(SchedaConto table)
        {

        }
    }
}
