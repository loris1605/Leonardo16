namespace Models.Repository
{
    public class EntraSocioInputR : RepositoryOld, IDisposable
    {
        public EntraSocioInputR() : base() { }
        public EntraSocioInputR(string connectionstring) : base(connectionstring) { }

        static int deadentries;
        ~EntraSocioInputR()
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
