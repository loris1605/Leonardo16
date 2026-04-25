using Microsoft.Data.SqlClient;
using Models.Entity;
using Models.StoreProcedure;
using SysNet;
using System.Data;

namespace Models.Repository
{
    public class CassaPostazioneR : RepositoryOld, IDisposable
    {
        public CassaPostazioneR() : base() { }
        public CassaPostazioneR(string connectionstring) : base(connectionstring) { }

        static int deadentries;

        private readonly CassaPostazioneSP sp = Create<CassaPostazioneSP>.Instance();

        ~CassaPostazioneR()
        {
            Interlocked.Increment(ref deadentries);
            _ = DisposeR.WriteDisposeAsync(this.GetType().Name + " #" + deadentries.ToString());

        }

        public SchedaMap GetSchedaAperta(int posizione)
        {
            var list = GetData<SchedaMap>(sp.SchedaGetByPosizione, posizione);
            if (list == null || list.Count == 0) return null;
            return list[0];
        }

        public bool IfExistPosizione(int nuovaposizione, SchedaMap dT)
        {
            return IfExistUpd(nuovaposizione, dT.Id, sp.SchedaEsistePosizioneUpd);
        }

        public bool UpdPosizione(SchedaMap map, int nuovaposizione)
        {
            SetDbParam(sp.SchedaUpdatePosizione);

            Cmd.Parameters.Add(new SqlParameter("@param1", map.Id));
            Cmd.Parameters.Add(new SqlParameter("@param2", nuovaposizione));
            
            SqlParameter outparam = new("@result", SqlDbType.Bit)
            {
                Direction = ParameterDirection.Output
            };

            Cmd.Parameters.Add(outparam);

            bool result = false;

            try
            {
                Conn.Open();
                Cmd.ExecuteNonQuery();

                result = (bool)outparam.Value;

            }
            catch (Exception)
            {
                result = false;
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
