using Models.Entity;

namespace Models.Repository
{
    public interface IMenuR : IDisposable
    {
        List<PostazioneMap> CaricaPostazioniCassa();
        bool EsisteGiornataAperta();
    }
}