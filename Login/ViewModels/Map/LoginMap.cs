using DTO.Entity;
using ReactiveUI;

namespace ViewModels.BindableObjects
{
    public class LoginMap : BindableMap
    {
        public LoginMap() { }

        public LoginMap(LoginDTO dto)
        {
            this.Id = dto.Id;
            this.NomeOperatore = dto.NomeOperatore;
            this.Password = dto.Password;
        }

        public LoginDTO ToDto()
        {
            return new LoginDTO
            {
                Id = this.Id,
                NomeOperatore = this.NomeOperatore,
                Password = this.Password
            };
        }
        
        private string _nomeoperatore;
        public string NomeOperatore
        {
            get => _nomeoperatore;
            set => this.RaiseAndSetIfChanged(ref _nomeoperatore, value);
        }

        private string _password;
        public string Password
        {
            get => _password;
            set => this.RaiseAndSetIfChanged(ref _password, value);
        }

        public override string Titolo => $"Login: {NomeOperatore}";

        
    }
}
