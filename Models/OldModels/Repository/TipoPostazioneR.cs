using Models.Entity;

namespace Models.Repository
{
    public class TipoPostazioneR : RepositoryOld, IDisposable
    {
        public TipoPostazioneR() : base() { }
        public TipoPostazioneR(string connectionstring) : base(connectionstring) { }



#if DEBUG
        static int deadentries;
        ~TipoPostazioneR()
        {
            Interlocked.Increment(ref deadentries);
            _ = DisposeR.WriteDisposeAsync(this.GetType().Name + " #" + deadentries.ToString());

        }
#endif
        public override void Dispose() => base.Dispose();

        public List<TipoPostazioneMap> Load() => GetData<TipoPostazioneMap>("TipoPostazioneMapGetData");
    }
}
