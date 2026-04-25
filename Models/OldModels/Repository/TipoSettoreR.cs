using Models.Entity;
using Models.StoreProcedure;
using SysNet;

namespace Models.Repository
{
    public class TipoSettoreR : RepositoryOld, IDisposable
    {
        public TipoSettoreR() : base() { }
        public TipoSettoreR(string connectionstring) : base(connectionstring) { }

        private readonly TipoSettoreSP sp = Create<TipoSettoreSP>.Instance();

        
#if DEBUG
        static int deadentries;
        ~TipoSettoreR()
        {
            Interlocked.Increment(ref deadentries);
            _ = DisposeR.WriteDisposeAsync(this.GetType().Name + " #" + deadentries.ToString());

        }
#endif

        public override void Dispose() => base.Dispose();

        public List<TipoSettoreMap> Load() => GetData<TipoSettoreMap>(sp.TipoSettoreMapGetData);
    }
}
