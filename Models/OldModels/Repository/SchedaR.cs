using Models.Entity;
using Models.Interfaces;

namespace Models.Repository
{
    public class SchedaR : RepositoryOld, IGroupQ<SchedaMap>
    {
        public SchedaR() : base() { }
        public SchedaR(string connectionstring) : base(connectionstring) { }

        static int deadentries;
        ~SchedaR()
        {
            Interlocked.Increment(ref deadentries);
            _ = DisposeR.WriteDisposeAsync(this.GetType().Name + " #" + deadentries.ToString());
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public List<SchedaMap> Load(int index = 0) => null;
        

        public List<SchedaMap> LoadByModel(object model)
        {
            throw new NotImplementedException();
        }
    }
}
