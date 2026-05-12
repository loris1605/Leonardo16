using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels.BindableObjects
{
    public class EntraSocioMap : BindableMap
    {
        public EntraSocioMap() { }




        private string _numeroTessera = string.Empty;
        public string NumeroTessera
        {
            get => _numeroTessera;
            set => this.RaiseAndSetIfChanged(ref _numeroTessera, value);
        }

        private string _cognome;
        public string Cognome
        {
            get => _cognome;
            set => this.RaiseAndSetIfChanged(ref _cognome, value);
        }

        private string nome = string.Empty;
        public override string Nome
        {
            get => nome;
            set => this.RaiseAndSetIfChanged(ref nome, value);

        }

        private int _natoil;
        public int Natoil
        {
            get => _natoil;
            set => this.RaiseAndSetIfChanged(ref _natoil, value);

        }

        private int _codicesocio;
        public int CodiceSocio
        {
            get => _codicesocio;
            set => this.RaiseAndSetIfChanged(ref _codicesocio, value);

        }

        private string numerosocio = string.Empty;
        public string NumeroSocio
        {
            get => numerosocio;
            set => this.RaiseAndSetIfChanged(ref numerosocio, value);

        }

        private int _codicetessera;
        public int CodiceTessera
        {
            get => _codicetessera;
            set => this.RaiseAndSetIfChanged(ref _codicetessera, value);

        }


        private int _scadenza;
        public int Scadenza
        {
            get => _scadenza;
            set => this.RaiseAndSetIfChanged(ref _scadenza, value);

        }

        private string _codiceunivoco = string.Empty;
        public string CodiceUnivoco
        {
            get => _codiceunivoco;
            set => this.RaiseAndSetIfChanged(ref _codiceunivoco, value);

        }

    }
}
