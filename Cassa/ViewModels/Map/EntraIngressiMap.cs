using DTO.Entity;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels.BindableObjects
{
    public class EntraIngressiMap : BindableMap
    {
        public EntraIngressiMap() { }

        public EntraIngressiMap(EntraIngressiDTO dto)
        {
            this.Id = dto.Id;
            this.NomeTariffa = dto.NomeTariffa;
            this.EtichettaTariffa = dto.EtichettaTariffa;
            this.PrezzoTariffa = dto.PrezzoTariffa;
            this.IsFreeDrink = dto.IsFreeDrink;
        }



        private string _nometariffa = string.Empty;
        public string NomeTariffa
        {
            get => _nometariffa;
            set => this.RaiseAndSetIfChanged(ref _nometariffa, value);
        }

        private string _etichettatariffa = string.Empty;
        public string EtichettaTariffa
        {
            get => _etichettatariffa;
            set => this.RaiseAndSetIfChanged(ref _etichettatariffa, value);
        }

        private decimal _prezzotariffa = 0M;
        public decimal PrezzoTariffa
        {
            get => _prezzotariffa;
            set => this.RaiseAndSetIfChanged(ref _prezzotariffa, value);
        }

        private bool _isfreedrink;
        public bool IsFreeDrink
        {
            get => _isfreedrink;
            set => this.RaiseAndSetIfChanged(ref _isfreedrink, value);
        }

    }
}
