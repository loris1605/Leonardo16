using Microsoft.Data.SqlClient;
using Models.Entity;
using Models.StoreProcedure;
using SysNet;
using System.Data;

namespace Models.Repository
{
    public class TesseraR : RepositoryOld, IDisposable, ITesseraR
    {
        public override void Dispose()
        {
            base.Dispose();
        }

        public TesseraR() : base() { }
        public TesseraR(string connectionstring) : base(connectionstring) { }

        private readonly PersonSP sp = Create<PersonSP>.Instance();

        static int deadentries;

#if DEBUG
        ~TesseraR()
        {
            Interlocked.Increment(ref deadentries);
            _ = DisposeR.WriteDisposeAsync(this.GetType().Name + " #" + deadentries.ToString());

        }
#endif
        public bool EsisteNumeroTessera(int numerotessera) => IfExist(sp.TesseraEsisteNumero, numerotessera);
        public bool EsisteNumeroTesseraUpd(PersonMap dT) =>
                                IfExistUpd(dT.NumeroTessera, dT.CodiceTessera, sp.TesseraEsisteNumeroUpd);

        public PersonMap SocioFirst(int codicesocio)
        {
            var list = GetData<PersonMap>(sp.SocioGetById, codicesocio);
            if (list == null || list.Count == 0) return null;
            return list[0];
        }
        public PersonMap TesseraFirst(int codicetessera)
        {
            var list = GetData<PersonMap>(sp.TesseraGetById, codicetessera);
            if (list == null || list.Count == 0) return null;
            return list[0];

        }
        public int Add(PersonMap map)
        {
            SetDbParam(sp.TesseraInsertData);

            Cmd.Parameters.Add(new SqlParameter("@socioid", map.CodiceSocio));
            Cmd.Parameters.Add(new SqlParameter("@numcard", map.NumeroTessera));
            Cmd.Parameters.Add(new SqlParameter("@scadenza", map.ScadenzaDate.DateToInt()));

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
        public bool Del(int id) => Del(id, sp.TesseraDeleteData);
        public bool Upd(PersonMap map)
        {
            SetDbParam(sp.TesseraUpdateData);

            Cmd.Parameters.Add(new SqlParameter("@id", map.CodiceTessera));
            Cmd.Parameters.Add(new SqlParameter("@numcard", map.NumeroTessera));
            Cmd.Parameters.Add(new SqlParameter("@scadenza", map.ScadenzaDate.DateToInt()));
            Cmd.Parameters.Add(new SqlParameter("@socioid", map.CodiceSocio));


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
