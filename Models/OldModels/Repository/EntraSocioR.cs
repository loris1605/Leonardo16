namespace Models.Repository
{
    public class EntraSocioR : RepositoryOld, IDisposable
    {
        public EntraSocioR() : base() { }
        public EntraSocioR(string connectionstring) : base(connectionstring) { }

        static int deadentries;
        ~EntraSocioR()
        {
            Interlocked.Increment(ref deadentries);
            _ = DisposeR.WriteDisposeAsync(this.GetType().Name + " #" + deadentries.ToString());
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
