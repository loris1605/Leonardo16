using Microsoft.Data.SqlClient;
using Models.Entity;
using Models.Interfaces;
using Models.StoreProcedure;
using SysNet;
using System.Data;

namespace Models.Repository
{
    public partial class SettoreR : RepositoryOld, IDisposable, IGroupQ<SettoreMap>
    {
        public SettoreR() : base() { }
        public SettoreR(string connectionstring) : base(connectionstring) { }

#if DEBUG
        static int deadentries;
        ~SettoreR()
        {
            Interlocked.Increment(ref deadentries);
            _ = DisposeR.WriteDisposeAsync(this.GetType().Name + " #" + deadentries.ToString());
        }
#endif

        private readonly SettoreSP sp = Create<SettoreSP>.Instance();

        public override void Dispose() => base.Dispose();

        public List<SettoreMap> Load(int index = 0) =>
                            index == 0 ?
                            GetData<SettoreMap>(sp.SettoreMapGetData) :
                            GetData<SettoreMap>(sp.SettoreMapGetById, index);

        public List<SettoreMap> LoadByModel(object model) => null;

        public SettoreMap First(int id)
        {
            List<SettoreMap> list = Load(id);
            if (list == null || list.Count == 0) return null;
            return list[0];
        }

        public bool IfExistName(SettoreMap dT) => IfExist(sp.SettoreEsisteNome, dT.NomeSettore);
        public bool IfExistName(string nomesettore, int codicesettore) =>
                            IfExistUpd(nomesettore, codicesettore, sp.SettoreEsisteNomeUpd);

        public bool IfExistReparti(SettoreMap dT) => IfExist(sp.RepartoEsisteSettore, dT.Id);
        public bool IfExistListini(SettoreMap dT) => IfExist(sp.ListinoEsisteSettore, dT.Id);

        public int Add(SettoreMap map)
        {
            SetDbParam(sp.SettoreInsertData);

            Cmd.Parameters.Add(new SqlParameter("@nomesettore", map.NomeSettore));
            Cmd.Parameters.Add(new SqlParameter("@labelsettore", map.EtichettaSettore));
            Cmd.Parameters.Add(new SqlParameter("@tiposettoreid", map.CodiceTipoSettore));

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
                
            }
            finally
            {
                Conn.Close();
                Conn.Dispose();
                Cmd.Dispose();
                
            }


            return result;
        }
        public bool Del(SettoreMap map) => Del(map.Id, sp.SettoreDeleteData);
        public bool Upd(SettoreMap map)
        {
            SetDbParam(sp.SettoreUpdateData);

            Cmd.Parameters.Add(new SqlParameter("@id", map.Id));
            Cmd.Parameters.Add(new SqlParameter("@nomesettore", map.NomeSettore));
            Cmd.Parameters.Add(new SqlParameter("@labelsettore", map.EtichettaSettore));
            Cmd.Parameters.Add(new SqlParameter("@tiposettoreid", map.CodiceTipoSettore));

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
                Cmd.Dispose();
            }


            return result;
        }
    }
}
