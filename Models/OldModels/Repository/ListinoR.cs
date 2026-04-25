using Microsoft.Data.SqlClient;
using Models.Entity;
using Models.StoreProcedure;
using SysNet;
using System.Data;

namespace Models.Repository
{
    public class ListinoR : RepositoryOld, IDisposable
    {
        public ListinoR() : base() { }
        public ListinoR(string connectionstring) : base(connectionstring) { }

        static int deadentries;

        private readonly ListinoSP sp = Create<ListinoSP>.Instance();

        ~ListinoR()
        {
            Interlocked.Increment(ref deadentries);
            _ = DisposeR.WriteDisposeAsync(this.GetType().Name + " #" + deadentries.ToString());

        }

        public List<TariffaElencoMap> LoadTariffe(int codicesettore)
        {
            return GetData<TariffaElencoMap>(sp.TariffaElencoMapGetData, codicesettore);
        }

        public bool Del(int settoreid, int tariffaid)
        {
            SetDbParam(sp.ListinoDeleteBySettoreByTariffa);
            Cmd.Parameters.Add(new SqlParameter("@param1", settoreid));
            Cmd.Parameters.Add(new SqlParameter("@param2", tariffaid));

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

        public int Add(int idsettore, int idtariffa)
        {
            SetDbParam(sp.ListinoInsertData);

            Cmd.Parameters.Add(new SqlParameter("@idsettore", idsettore));
            Cmd.Parameters.Add(new SqlParameter("@idtariffa", idtariffa));

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
