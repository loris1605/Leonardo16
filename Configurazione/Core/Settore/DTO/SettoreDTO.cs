using Models.Interfaces;
using Models.Tables;
using System.Linq.Expressions;

namespace DTO.Entity
{
    public class SettoreDTO : BaseDTO, IMap, IMappable<Settore>
    {
        public string NomeSettore { get; set; } = string.Empty;
        public string EtichettaSettore { get; set; } = string.Empty;
        public int CodiceTipoSettore { get; set; } = 0;
        public string NomeTipoSettore { get; set; } = string.Empty;
        public int CodiceListino { get; set; } = 0;
        public string NomeTariffa { get; set; } = string.Empty;
        public string EtichettaTariffa { get; set; } = string.Empty;
        public decimal PrezzoTariffa { get; set; } = decimal.Zero;
        public bool HasReparto { get; set; }

        public override string Nome => NomeSettore;

        public override string Titolo => $"{NomeSettore} - {NomeTipoSettore}";

        public SettoreDTO() { }

        public SettoreDTO(Settore table)
        {
            this.Id = table.Id;
            this.NomeSettore = table.Nome;
            this.EtichettaSettore = table.Label;
            this.CodiceTipoSettore = table.TipoSettoreId;
        }

        public Settore ToTable()
        {
            return new Settore
            {
                Id = this.Id,
                Nome = this.NomeSettore,
                Label = this.EtichettaSettore,
                TipoSettoreId = this.CodiceTipoSettore
            };
        }

        public void UpdateTable(Settore existing)
        {
            if (existing == null) return;
            // Aggiorniamo solo i campi che possono cambiare
            existing.Nome = this.NomeSettore;
            existing.Label = this.EtichettaSettore;
            existing.TipoSettoreId = this.CodiceTipoSettore;

            // Non tocchiamo l'ID!
        }

        public static Expression<Func<Settore, SettoreDTO>> ToSettoreDto => entity => new SettoreDTO
        {
            Id = entity.Id,
            NomeSettore = entity.Nome,
            EtichettaSettore = entity.Label,
            CodiceTipoSettore = entity.TipoSettoreId
        };

        public static Expression<Func<Settore, Listino, SettoreDTO>> ToSettoriDtoRelationed =>
        (o, p) => new SettoreDTO
        {
            Id = o.Id,
            // Protezione su TipoPostazione
            CodiceTipoSettore = o.TipoSettore != null ? o.TipoSettore.Id : 0,
            NomeSettore = o.Nome,
            EtichettaSettore = o.Label,
            NomeTipoSettore = o.TipoSettore != null ? o.TipoSettore.Nome : "N/A",

            // Protezione su Reparto
            CodiceListino = p != null ? p.Id : 0,

            // Protezione a cascata su Settore (p -> p.Settore)
            NomeTariffa = (p != null && p.Tariffa != null) ? p.Tariffa.Nome : "Nessuno",
            EtichettaTariffa = (p != null && p.Tariffa != null) ? p.Tariffa.Label : "Nessuna",
            PrezzoTariffa = (p != null && p.Tariffa != null) ? p.Tariffa.Prezzo : 0M,

            HasReparto = o.Reparti.Any()
        };
    }
}
