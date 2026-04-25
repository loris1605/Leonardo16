using Microsoft.Data.SqlClient;
using Models.Entity;
using Models.StoreProcedure;
using SysNet;
using System.Data;

namespace Models.Repository
{
    public class RepartoR : RepositoryOld, IDisposable
    {
        public RepartoR() : base() { }
        public RepartoR(string connectionstring) : base(connectionstring) { }

        static int deadentries;

        private readonly RepartoSP sp = Create<RepartoSP>.Instance();

        ~RepartoR()
        {
            Interlocked.Increment(ref deadentries);
            _ = DisposeR.WriteDisposeAsync(this.GetType().Name + " #" + deadentries.ToString());

        }

        public List<SettoreElencoMap> LoadSettori(int codicepostazione)
        {
            return GetData<SettoreElencoMap>(sp.SettoreElencoMapGetData, codicepostazione);
        }

        public bool Del(int postazioneid, int settoreid)
        {
            SetDbParam(sp.RepartoDeleteByPostazioneBySettore);
            Cmd.Parameters.Add(new SqlParameter("@param1", postazioneid));
            Cmd.Parameters.Add(new SqlParameter("@param2", settoreid));

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

        public int Add(int idpostazione, int idsettore)
        {
            SetDbParam(sp.RepartoInsertData);

            Cmd.Parameters.Add(new SqlParameter("@idpostazione", idpostazione));
            Cmd.Parameters.Add(new SqlParameter("@idsettore", idsettore));

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
