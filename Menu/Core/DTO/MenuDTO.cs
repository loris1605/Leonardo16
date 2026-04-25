using DTO.Entity;
using Models.Interfaces;
using Models.Tables;
using System.Linq.Expressions;

namespace Menu.Core.DTO
{
    public class MenuDTO : BaseDTO, IMap, IMappable<Postazione>
    {
        public int CodiceTipoPostazione { get; set; }
        public string NomePostazione { get; set; } = string.Empty;
        public string NomeTipoPostazione { get; set; } = string.Empty;
        public int CodiceReparto { get; set; }
        public string NomeSettore { get; set; } = string.Empty;
        public string EtichettaSettore { get; set; } = string.Empty;
        public string NomeTipoSettore { get; set; } = string.Empty;
        public int CodiceTipoRientro { get; set; }
        public string NomeTipoRientro { get; set; } = string.Empty;
        public bool HasPermesso { get; set; }

        public override string Nome
        {
            get => NomePostazione;
            set => NomePostazione = value ?? string.Empty;
        }

        public override string Titolo => $"{NomePostazione} - {NomeTipoPostazione}";

        public MenuDTO() { }

        public MenuDTO(Postazione table)
        {
            this.Id = table.Id;
            this.NomePostazione = table.Nome;
            this.CodiceTipoPostazione = table.TipoPostazioneId;
            this.CodiceTipoRientro = table.TipoRientroId;
        }

        public Postazione ToTable()
        {
            return new Postazione
            {
                Id = this.Id,
                Nome = this.NomePostazione,
                TipoPostazioneId = this.CodiceTipoPostazione,
                TipoRientroId = this.CodiceTipoRientro
            };
        }

        public void UpdateTable(Postazione existing)
        {
            if (existing == null) return;
            // Aggiorniamo solo i campi che possono cambiare
            existing.Nome = this.NomePostazione;
            existing.TipoPostazioneId = this.CodiceTipoPostazione;
            existing.TipoRientroId = this.CodiceTipoRientro;

            // Non tocchiamo l'ID!
        }

        public static Expression<Func<Postazione, MenuDTO>> ToPostazioneDto => entity => new MenuDTO
        {
            Id = entity.Id,
            NomePostazione = entity.Nome,
            CodiceTipoPostazione = entity.TipoPostazioneId,
            CodiceTipoRientro = entity.TipoRientroId
        };


        public static Expression<Func<Postazione, Reparto, MenuDTO>> ToPostazioniDtoRelationed =>
        (o, p) => new MenuDTO
        {
            Id = o.Id,
            // Protezione su TipoPostazione
            CodiceTipoPostazione = o.TipoPostazione != null ? o.TipoPostazione.Id : 0,
            NomePostazione = o.Nome,
            NomeTipoPostazione = o.TipoPostazione != null ? o.TipoPostazione.Nome : "N/A",

            // Protezione su Reparto
            CodiceReparto = p != null ? p.Id : 0,

            // Protezione a cascata su Settore (p -> p.Settore)
            NomeSettore = (p != null && p.Settore != null) ? p.Settore.Nome : "Nessuno",
            EtichettaSettore = (p != null && p.Settore != null) ? p.Settore.Label : "Nessuna",

            // Protezione profonda (p -> p.Settore -> p.Settore.TipoSettore)
            NomeTipoSettore = (p != null && p.Settore != null && p.Settore.TipoSettore != null)
                              ? p.Settore.TipoSettore.Nome
                              : "N/A",
            HasPermesso = o.Permessi.Any()
        };

        public static Expression<Func<Permesso, MenuDTO>> ToPermessoDTO => p => new MenuDTO
        {
            Id = p.Id,
            // Attenzione: qui mappi PostazioneId su CodiceTipoPostazione, 
            // verifica che non debba essere p.Postazione.TipoPostazioneId
            CodiceTipoPostazione = p.PostazioneId,
            NomePostazione = p.Postazione != null ? p.Postazione.Nome : "N/A"
        };


    }
}
