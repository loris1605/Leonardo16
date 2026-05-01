using Models.Interfaces;
using Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Entity
{
    public class SettoreElencoDTO : BaseDTO, IMap
    {
        public string NomeSettore { get; set; } = string.Empty;
        public string EtichettaSettore { get; set; } = string.Empty;
        public int CodiceTipoSettore { get; set; } = 0;
        public string NomeTipoSettore { get; set; } = string.Empty;
        public bool HasReparto { get; set; }

        public override string Nome => NomeSettore;

        public SettoreElencoDTO() { }

        public static Expression<Func<Settore, SettoreElencoDTO>> ToSettoreElencoDto(int postazioneId)
        {
            return p => new SettoreElencoDTO
            {
                Id = p.Id,
                NomeSettore = p.Nome,
                EtichettaSettore = p.Label,
                CodiceTipoSettore = p.TipoSettoreId,
                NomeTipoSettore = p.TipoSettore!.Nome,
                // Usiamo il parametro del metodo (postazioneId) dentro l'espressione
                HasReparto = p.Reparti.Any(r => r.PostazioneId == postazioneId  )
            };
        }
    }
}
