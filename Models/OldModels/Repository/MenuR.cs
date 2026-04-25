using Models.Entity;
using Models.Entity.Global;
using Models.StoreProcedure;
using SysNet;

namespace Models.Repository
{
    public partial class MenuR : RepositoryOld, IDisposable, IMenuR
    {
        public MenuR() : base() { }

        public MenuR(string connectionstring) : base(connectionstring) { }

        private readonly MenuSP sp = Create<MenuSP>.Instance();

        static int deadentries;

#if DEBUG
        ~MenuR()
        {

            Interlocked.Increment(ref deadentries);
            _ = DisposeR.WriteDisposeAsync(this.GetType().Name + " #" + deadentries.ToString());

        }

#endif

        public override void Dispose() => base.Dispose();

        public bool EsisteGiornataAperta() => IfExist(sp.EsisteGiornataAperta);

        public List<PostazioneMap> CaricaPostazioniCassa() =>
            GetData<PostazioneMap>(sp.PostazioniCassaGetByOperatore, GlobalValuesC.MySetting.IDOPERATORE);
    }
}
