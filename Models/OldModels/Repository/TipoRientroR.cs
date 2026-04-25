using Microsoft.Data.SqlClient;
using Models.Entity;
using Models.Interfaces;
using Models.StoreProcedure;
using SysNet;
using System.Data;

namespace Models.Repository
{
    public class TipoRientroR : RepositoryOld, IGroupQ<TipoRientroMap>, IDisposable
    {
        public TipoRientroR() : base() { }
        public TipoRientroR(string connectionstring) : base(connectionstring) { }

        static int deadentries;

#if DEBUG
        ~TipoRientroR()
        {
            Interlocked.Increment(ref deadentries);
            _ = DisposeR.WriteDisposeAsync(this.GetType().Name + " #" + deadentries.ToString());

        }
#endif

        private readonly TipoRientroSP sp = Create<TipoRientroSP>.Instance();
        public override void Dispose() => base.Dispose();

        public List<TipoRientroMap> Load(int index = 0) => index == 0 ?
                            GetData<TipoRientroMap>(sp.TipoRientroMapGetData) :
                            GetData<TipoRientroMap>(sp.TipoRientroMapGetById, index);

        public TipoRientroMap First(int id)
        {
            List<TipoRientroMap> list = Load(id);
            if (list == null || list.Count == 0) return null;
            return list[0];
        }


        public List<TipoRientroMap> LoadByModel(object model) => null;

        public bool IfExistName(TipoRientroMap dT) => IfExist(sp.TipoRientroEsisteNome, dT.Nome);
        public bool IfExistTipoRientro(TipoRientroMap dT) => IfExist(sp.PostazioneEsisteTipoRientro, dT.Id);
        public bool IfExistName(string nometiporientro, int codicetiporientro) =>
                            IfExistUpd(nometiporientro, codicetiporientro, sp.TipoRientroEsisteNomeUpd);

        public int Add(TipoRientroMap map)
        {
            SetDbParam(sp.TipoRientroInsertData);

            Cmd.Parameters.Add(new SqlParameter("@nometiporientro", map.Nome));
            Cmd.Parameters.Add(new SqlParameter("@durata", map.Hours));

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

        public bool Del(TipoRientroMap map) => Del(map.Id, sp.TipoRientroDeleteData);
        public bool Upd(TipoRientroMap map)
        {
            SetDbParam(sp.TipoRientroUpdateData);

            Cmd.Parameters.Add(new SqlParameter("@id", map.Id));
            Cmd.Parameters.Add(new SqlParameter("@nometiporientro", map.Nome));
            Cmd.Parameters.Add(new SqlParameter("@durataore", map.Hours));

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
