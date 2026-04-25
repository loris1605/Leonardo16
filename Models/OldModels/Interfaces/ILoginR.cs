using Models.Entity;

namespace Models.Repository
{
    public interface ILoginR : IDisposable
    {
        List<LoginMap> GetOperatoriAbilitati();
        void SaveSettings(LoginMap dT);
    }
}