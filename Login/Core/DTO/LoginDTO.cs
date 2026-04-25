using Models.Entity.Global;
using Models.Interfaces;
using Models.Tables;
using System.Linq.Expressions;

namespace DTO.Entity
{
    public class LoginDTO : BaseDTO, IMap
    {
        public string NomeOperatore { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        // Collega la proprietà Nome della base a NomeOperatore
        public override string Nome
        {
            get => NomeOperatore;
            set => NomeOperatore = value ?? string.Empty;
        }

        public override string Titolo => $"Login: {NomeOperatore}";

        public LoginDTO() { }

        public LoginDTO(Operatore table)
        {
            this.Id = table.Id;
            this.NomeOperatore = table.Nome;
            this.Password = table.Password;
        }

        public static Expression<Func<Operatore, LoginDTO>> ToLoginDto => entity => new LoginDTO
        {
            Id = entity.Id,
            NomeOperatore = entity.Nome,
            Password = entity.Password,
            
        };

        public static Expression<Func<Permesso, PostazioneXC>> ToPostazioneXC => p => new PostazioneXC
        {
            // Usiamo l'ID direttamente dal Permesso (più sicuro della navigazione)
            CODICEPOSTAZIONE = p.PostazioneId,
            // Usiamo il null-conditional per la navigazione
            DESCPOSTAZIONE = p.Postazione != null ? p.Postazione.Nome : "Nessuna",
            TIPOPOSTAZIONE = p.Postazione != null ? p.Postazione.TipoPostazioneId : 0
        };

        public static Expression<Func<Reparto, SettoreXC>> ToSettoreXC => r => new SettoreXC
        {
            // Usiamo l'ID direttamente dal Reparto (FK) per sicurezza
            CODICESETTORE = r.SettoreId,

            // Protezione contro i null per le proprietà di navigazione
            DESCSETTORE = r.Settore != null ? r.Settore.Label : "DESCRIZIONE MANCANTE",
            NOMESETTORE = r.Settore != null ? r.Settore.Nome : "SETTORE NON ASSEGNATO"
        };

        public static Expression<Func<Listino, TariffaXC>> ToTariffaXC => p => new TariffaXC
        {
            CODICETARIFFA = p.TariffaId,
            DESCTARIFFA = p.Tariffa!.Label,
            PRICETARIFFA = p.Tariffa.Prezzo
        };

        public static Expression<Func<Giornata, GiornataXC>> ToGiornataXC => p => new GiornataXC
        {
            IDGIORNATA = p.Id,
            DATAINIZIO = p.DataInizio,
            DATAFINE = p.DataFine,
            APERTA = p.Aperta
        };


    }
}
