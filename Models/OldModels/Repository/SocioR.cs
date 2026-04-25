using Microsoft.Data.SqlClient;
using Models.Entity;
using Models.StoreProcedure;
using SysNet;
using System.Data;

namespace Models.Repository
{
    public class SocioR : RepositoryOld, IDisposable, ISocioR
    {
        public SocioR() : base() { }
        public SocioR(string connectionstring) : base(connectionstring) { }

        private readonly PersonSP sp = Create<PersonSP>.Instance();

        static int deadentries;

#if DEBUG
        ~SocioR()
        {
            Interlocked.Increment(ref deadentries);
            _ = DisposeR.WriteDisposeAsync(this.GetType().Name + " #" + deadentries.ToString());

        }

#endif

        public override void Dispose() => base.Dispose();

        public bool EsisteNumeroSocio(string numerosocio) => IfExist(sp.SocioEsisteNumero, numerosocio);

        private List<PersonMap> Load(int index = 0) =>
                                                    index == 0 ?
                                                    GetData<PersonMap>(sp.SociGet) :
                                                    GetData<PersonMap>(sp.SocioGetById, index);


        public PersonMap First(int id)
        {
            List<PersonMap> list = Load(id);
            return list[0];
        }

        public PersonMap SocioFirst(int codicesocio)
        {
            var list = GetData<PersonMap>(sp.SocioGetById, codicesocio);
            if (list == null || list.Count == 0) return null;
            return list[0];
        }

        public bool HasCodiciTessera(int idsocio) => IfExist(sp.TesseraAnyBySocio, idsocio);

        public bool EsisteNumeroSocio(string numerosocio, int idsocio) =>
                                            IfExistUpd(numerosocio, idsocio, sp.SocioEsisteNumeroSocioUpd);

        public int Add(PersonMap map)
        {
            SetDbParam(sp.SocioInsertData);

            Cmd.Parameters.Add(new SqlParameter("@personid", map.Id));
            Cmd.Parameters.Add(new SqlParameter("@numsocio", map.NumeroSocio));

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
        public bool Del(int id) => Del(id, "SocioDeleteData");

        public bool Upd(PersonMap map)
        {
            SetDbParam(sp.SocioUpdateData);

            Cmd.Parameters.Add(new SqlParameter("@id", map.CodiceSocio));
            Cmd.Parameters.Add(new SqlParameter("@numsocio", map.NumeroSocio));

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
