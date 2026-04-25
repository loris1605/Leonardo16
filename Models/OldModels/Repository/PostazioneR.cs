using Microsoft.Data.SqlClient;
using Models.Entity;
using Models.Interfaces;
using Models.StoreProcedure;
using SysNet;
using System.Data;


namespace Models.Repository
{
    public partial class PostazioneR : RepositoryOld, IDisposable, IGroupQ<PostazioneMap>
    {
        public PostazioneR() : base() { }
        public PostazioneR(string connectionstring) : base(connectionstring) { }

#if DEBUG
        static int deadentries;
        ~PostazioneR()
        {
            Interlocked.Increment(ref deadentries);
            _ = DisposeR.WriteDisposeAsync(this.GetType().Name + " #" + deadentries.ToString());
            //MessageBox.Show("PersonR Destroyed");
        }
#endif
        public override void Dispose() => base.Dispose();

        private readonly PostazioneSP sp = Create<PostazioneSP>.Instance();

        public List<PostazioneMap> Load(int index = 0) =>
                            index == 0 ?
                            GetData<PostazioneMap>(sp.PostazioneMapGetData) :
                            GetData<PostazioneMap>(sp.PostazioneMapGetById, index);

        public List<PostazioneMap> LoadByModel(object model) => null;

        public bool IfExistName(PostazioneMap dT) => IfExist(sp.PostazioneEsisteNome, dT.NomePostazione);
        public bool IfExistName(string nomepostazione, int codicepostazione) =>
                            IfExistUpd(nomepostazione, codicepostazione, sp.PostazioneEsisteNomeUpd);
        public bool IfExistPermessi(PostazioneMap dT) => IfExist(sp.PermessoEsistePostazione, dT.Id);
        public bool IfExistReparti(PostazioneMap dT) => IfExist(sp.RepartoEsistePostazione, dT.Id);


        public PostazioneMap First(int id)
        {
            List<PostazioneMap> list = Load(id);
            if (list == null || list.Count == 0) return null;
            return list[0];
        }

        public int Add(PostazioneMap map)
        {
            SetDbParam(sp.PostazioneInsertData);

            Cmd.Parameters.Add(new SqlParameter("@nomepostazione", map.NomePostazione));
            Cmd.Parameters.Add(new SqlParameter("@tipopostazioneid", map.CodiceTipoPostazione));
            Cmd.Parameters.Add(new SqlParameter("@tiporientroid", map.CodiceTipoRientro));

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
        public bool Del(PostazioneMap map) => Del(map.Id, sp.PostazioneDeleteData);
        public bool Upd(PostazioneMap map)
        {
            SetDbParam(sp.PostazioneUpdateData);

            Cmd.Parameters.Add(new SqlParameter("@id", map.Id));
            Cmd.Parameters.Add(new SqlParameter("@nomepostazione", map.NomePostazione));
            Cmd.Parameters.Add(new SqlParameter("@tipopostazioneid", map.CodiceTipoPostazione));
            Cmd.Parameters.Add(new SqlParameter("@tiporientroid", map.CodiceTipoRientro));

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
