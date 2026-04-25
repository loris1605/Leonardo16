using Microsoft.Data.SqlClient;
using Models.Entity;
using Models.Interfaces;
using Models.StoreProcedure;
using SysNet;
using System.Data;

namespace Models.Repository
{
    public class TariffaR : RepositoryOld, IDisposable, IGroupQ<TariffaMap>
    {
        public TariffaR() : base() { }
        public TariffaR(string connectionstring) : base(connectionstring) { }

#if DEBUG
        static int deadentries;
        ~TariffaR()
        {
            Interlocked.Increment(ref deadentries);
            _ = DisposeR.WriteDisposeAsync(this.GetType().Name + " #" + deadentries.ToString());
        }
#endif

        private readonly TariffaSP sp = Create<TariffaSP>.Instance();

        public override void Dispose() => base.Dispose();

        public List<TariffaMap> Load(int index = 0) =>
                            index == 0 ?
                            GetData<TariffaMap>(sp.TariffaMapGetData) :
                            GetData<TariffaMap>(sp.TariffaMapGetById, index);

        public List<TariffaMap> LoadByModel(object model) => null;

        public TariffaMap First(int id)
        {
            List<TariffaMap> list = Load(id);
            if (list == null || list.Count == 0) return null;
            return list[0];
        }


        public bool IfExistName(TariffaMap dT) => IfExist(sp.TariffaEsisteNome, dT.NomeTariffa);
        public bool IfExistName(string nometariffa, int codicetariffa) =>
                            IfExistUpd(nometariffa, codicetariffa, sp.TariffaEsisteNomeUpd);

        public bool IfExistListini(TariffaMap dT) => IfExist(sp.ListinoEsisteTariffa, dT.Id);


        public int Add(TariffaMap map)
        {
            SetDbParam(sp.TariffaInsertData);

            Cmd.Parameters.Add(new SqlParameter("@nometariffa", map.NomeTariffa));
            Cmd.Parameters.Add(new SqlParameter("@labeltariffa", map.EtichettaTariffa));
            Cmd.Parameters.Add(new SqlParameter("@isfreedrink", map.IsFreeDrink));
            Cmd.Parameters.Add(new SqlParameter("@prezzo", map.PrezzoTariffa));

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
                Conn.Dispose();
                Cmd.Dispose();
                Conn = null;
                Cmd = null;
            }


            return result;
        }
        public bool Del(TariffaMap map) => Del(map.Id, sp.TariffaDeleteData);

        public bool Upd(TariffaMap map)
        {
            SetDbParam(sp.TariffaUpdateData);

            Cmd.Parameters.Add(new SqlParameter("@id", map.Id));
            Cmd.Parameters.Add(new SqlParameter("@nometariffa", map.NomeTariffa));
            Cmd.Parameters.Add(new SqlParameter("@labeltariffa", map.EtichettaTariffa));
            Cmd.Parameters.Add(new SqlParameter("@isfreedrink", map.IsFreeDrink));
            Cmd.Parameters.Add(new SqlParameter("@prezzo", map.PrezzoTariffa));

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
