using Microsoft.Data.SqlClient;
using Models.Entity;
using Models.StoreProcedure;
using SysNet;
using System.Data;

namespace Models.Repository
{
    public class PermessoR : RepositoryOld, IDisposable
    {
        public PermessoR() : base() { }
        public PermessoR(string connectionstring) : base(connectionstring) { }

        private readonly PermessoSP sp = Create<PermessoSP>.Instance();

#if DEBUG 
        static int deadentries;
        ~PermessoR()
        {
            Interlocked.Increment(ref deadentries);
            _ = DisposeR.WriteDisposeAsync(this.GetType().Name + " #" + deadentries.ToString());
        }
#endif

        public override void Dispose()
        {
            base.Dispose();
        }

        public List<PostazioneElencoMap> LoadPostazioni(int codiceoperatore)
        {
            return GetData<PostazioneElencoMap>(sp.PostazioneElencoMapGetData, codiceoperatore);
        }

        public bool Del(int operatoreid, int postazioneid)
        {
            SetDbParam(sp.PermessoDeleteByOperatoreByPostazione);
            Cmd.Parameters.Add(new SqlParameter("@param1", operatoreid));
            Cmd.Parameters.Add(new SqlParameter("@param2", postazioneid));

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
                Conn.Dispose();
                Conn = null;
                Cmd = null;
            }


            return result;
        }

        public int Add(int idoperatore, int idpostazione)
        {
            SetDbParam(sp.PermessoInsertData);

            Cmd.Parameters.Add(new SqlParameter("@idoperatore", idoperatore));
            Cmd.Parameters.Add(new SqlParameter("@idpostazione", idpostazione));


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
