using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysNet.Converters
{
    public static class Converters
    {
        public static bool IsLegalAge(this int dataNascita)
        {
            // Validazione base del formato YYYYMMDD
            if (dataNascita < 19000101 || dataNascita > 21001231) return false;

            // Calcolo età (assumendo che DateIntToEta restituisca un int)
            int eta = dataNascita.DateIntToEta();

            // Restituisce true se maggiorenne (>= 18)
            return eta >= 18;
        }

        public static int DateIntToEta(this int dataNascita)
        {
            // Validazione rapida
            if (dataNascita < 19000101 || dataNascita > 21001231) return 0;

            try
            {
                // Estrazione matematica (molto più veloce di Substring)
                int annoNascita = dataNascita / 10000;
                int meseNascita = (dataNascita % 10000) / 100;
                int giornoNascita = dataNascita % 100;

                DateTime oggi = DateTime.Today;
                int eta = oggi.Year - annoNascita;

                // Sottrai un anno se il compleanno non è ancora avvenuto quest'anno
                if (oggi.Month < meseNascita || (oggi.Month == meseNascita && oggi.Day < giornoNascita))
                {
                    eta--;
                }

                return eta < 0 ? 0 : eta;
            }
            catch
            {
                return 0; // Fallback sicuro in caso di data non valida (es. 31 aprile)
            }
        }

        public static DateTime DateIntToDate(this int intData)
        {
            // 1. Gestione esplicita dello zero (evita log inutili o controlli range)
            if (intData == 0) return DateTime.MinValue;

            // 2. Controllo range (abbiamo allargato un po' per sicurezza storica)
            if (intData < 17530101 || intData > 99991231) return DateTime.MinValue;

            int year = intData / 10000;
            int month = (intData % 10000) / 100;
            int day = intData % 100;

            // 3. Validazione dei componenti prima del costruttore (opzionale ma pulito)
            if (month < 1 || month > 12 || day < 1 || day > 31) return DateTime.MinValue;

            try
            {
                return new DateTime(year, month, day);
            }
            catch
            {
                // Gestisce casi come il 29 febbraio in anni non bisestili
                return DateTime.MinValue;
            }
        }




        public static int DateTimeToIntDate(this DateTime date)
        {
            // Aggiungi il controllo per DateTime.MaxValue se capita di usare date di fine validità lontane
            if (date == DateTime.MinValue || date == DateTime.MaxValue || date.Year < 1900 || date.Year > 2100)
                return 0;

            return (date.Year * 10000) + (date.Month * 100) + date.Day;
        }


        public static int DateTimeToIntDate(this DateTime? date)
        {
            // Se l'oggetto è null, restituisce 0
            if (!date.HasValue) return 0;

            return date.Value.DateTimeToIntDate();
        }

        public static DateTimeOffset DateIntToDateTimeOffset(this int intData)
        {
            // Validazione immediata del formato YYYYMMDD
            if (intData < 19000101 || intData > 21001231) return DateTimeOffset.MinValue;

            // Estrazione matematica (evita allocazioni di stringhe)
            int year = intData / 10000;
            int month = (intData % 10000) / 100;
            int day = intData % 100;

            try
            {
                // Il costruttore richiede (anno, mese, giorno, ora, min, sec, offset)
                // Usiamo TimeSpan.Zero per UTC o l'offset locale corrente
                return new DateTimeOffset(year, month, day, 0, 0, 0, TimeSpan.Zero);
            }
            catch
            {
                // Fallback per date non valide (es. 20230230)
                return DateTimeOffset.MinValue;
            }
        }

        public static int DateTimeOffsetToDateInt(this DateTimeOffset dto)
        {
            // Se è il valore minimo o fuori dal range gestito dal tuo DB
            if (dto == DateTimeOffset.MinValue || dto.Year < 1900 || dto.Year > 2100) return 0;

            // Calcolo matematico diretto: YYYY * 10000 + MM * 100 + DD
            return (dto.Year * 10000) + (dto.Month * 100) + dto.Day;
        }

        public static int DateTimeOffsetToDateInt(this DateTimeOffset? dto)
        {
            // Se l'oggetto è null, restituiamo 0 (coerente con il tuo sistema)
            if (!dto.HasValue) return 0;

            // Chiamata al metodo sopra tramite .Value
            return dto.Value.DateTimeOffsetToDateInt();
        }


    }

    public static class  Services
    {
        public static void FireAndForget(this Task task)
        {
            task.ContinueWith(t => {
                if (t.IsFaulted) { /* Logga l'errore qui */ }
            }, TaskContinuationOptions.OnlyOnFaulted);
        }
    }
}
