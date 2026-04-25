using Models.Entity;
using Models.StoreProcedure;
using SysNet;

namespace Models.Repository
{
    public class GuardarobaR : RepositoryOld, IDisposable
    {
        public GuardarobaR() : base() { }
        public GuardarobaR(string connectionstring) : base(connectionstring) { }

        static int deadentries;
        ~GuardarobaR()
        {
            Interlocked.Increment(ref deadentries);
            _ = DisposeR.WriteDisposeAsync(this.GetType().Name + " #" + deadentries.ToString());

        }
        private readonly GuardarobaSP sp = Create<GuardarobaSP>.Instance();

        public GuardarobaMap CaricaBySchedaId(int codicescheda)
        {
            List<GuardarobaMap> list = GetData<GuardarobaMap>(sp.GuardarobaMapGetById, codicescheda);
            if (list == null || list.Count == 0) return null;
            return list[0];
        }
    }
}
