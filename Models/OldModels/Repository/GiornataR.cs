using Microsoft.Data.SqlClient;
using Models.StoreProcedure;
using SysNet;
using System.Data;

namespace Models.Repository
{
    public class GiornataR : RepositoryOld, IDisposable
    {
        public GiornataR() : base() { }
        public GiornataR(string connectionstring) : base(connectionstring) { }
        static int deadentries;

        ~GiornataR()
        {
            Interlocked.Increment(ref deadentries);
            _ = DisposeR.WriteDisposeAsync(this.GetType().Name + " #" + deadentries.ToString());

        }

        private readonly GiornataSP sp = Create<GiornataSP>.Instance();

        public int NuovaGiornata(DateTime adesso)
        {
            SetDbParam(sp.GiornataInsertData);

            Cmd.Parameters.Add(new SqlParameter("@datainizio", adesso.Ticks));
            Cmd.Parameters.Add(new SqlParameter("@datafine", DateTime.Parse("01/01/2100").Ticks));
            Cmd.Parameters.Add(new SqlParameter("@aperta", true));

            SqlParameter outparam = new("@result", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };

            Cmd.Parameters.Add(outparam);

            int result = 0;

            try
            {
                Conn.Open();
                Cmd.ExecuteNonQuery();
                if (outparam.Value is not DBNull)
                {
                    result = (int)outparam.Value;
                }


            }
            catch (Exception)
            {
                result = -1;
                throw;
            }
            finally
            {
                Conn.Close();
                Conn = null;
                Cmd = null;
            }


            return result;
        }
    }
}
