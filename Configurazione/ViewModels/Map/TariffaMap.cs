using DTO.Entity;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels.BindableObjects
{
    public class TariffaMap : BindableMap
    {
        public override string Nome => NomeTariffa;

        public TariffaMap() { }

        public TariffaMap(TariffaDTO dto)
        {
            this.Id = dto.Id;
            this.NomeTariffa = dto.NomeTariffa;
            this.EtichettaTariffa = dto.EtichettaTariffa;
            this.PrezzoTariffa = dto.PrezzoTariffa;
            this.HasListino = dto.HasListino;
            this.IsFreeDrink = dto.IsFreeDrink;
        }

        public TariffaDTO ToDto()
        {
            return new TariffaDTO
            {
                Id = this.Id,
                NomeTariffa = this.NomeTariffa,
                EtichettaTariffa = this.EtichettaTariffa,
                PrezzoTariffa = this.PrezzoTariffa,
                HasListino = this.HasListino,
                IsFreeDrink = this.IsFreeDrink
            };
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

        private bool _haslistino;
        public bool HasListino
        {
            get => _haslistino;
            set => this.RaiseAndSetIfChanged(ref _haslistino, value);
        }

        private bool _isfreedrink;
        public bool IsFreeDrink
        {
            get => _isfreedrink;
            set => this.RaiseAndSetIfChanged(ref _isfreedrink, value);
        }

        public override string Titolo => $"{NomeTariffa} - " +
            $"{PrezzoTariffa:C2} {(IsFreeDrink ? "(Free Drink)" : "")}";
    }
}
