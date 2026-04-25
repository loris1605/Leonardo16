using Microsoft.Data.SqlClient;
using Models.Entity;
using Models.StoreProcedure;
using SysNet;
using System.Data;

namespace Models.Repository
{
    public partial class OperatoreR : RepositoryOld, IOperatoreR
    {
        public OperatoreR() : base() { }
        public OperatoreR(string connectionstring) : base(connectionstring) { }

        private readonly OperatoreSP sp = Create<OperatoreSP>.Instance();

#if DEBUG
        static int deadentries;
        ~OperatoreR()
        {
            Interlocked.Increment(ref deadentries);
            _ = DisposeR.WriteDisposeAsync(this.GetType().Name + " #" + deadentries.ToString());
            //MessageBox.Show("PersonR Destroyed");
        }
#endif
        public override void Dispose() => base.Dispose();

        public List<OperatoreMap> Load(int index = 0) =>
                        index == 0 ?
                            GetData<OperatoreMap>(sp.OperatoriMapGet) :
                            GetData<OperatoreMap>(sp.OperatoriMapGetById, index);

        public List<OperatoreMap> LoadByModel(object model) => (List<OperatoreMap>)model;

        public OperatoreMap First(int id)
        {
            List<OperatoreMap> list = Load(id);
            if (list == null || list.Count == 0) return null;
            return list[0];
        }

        public bool IfExistNickname(OperatoreMap dT) => IfExist(sp.OperatoreEsisteNickname, dT.NomeOperatore);
        public bool IfExistNickname(string nomeoperatore, int codiceoperatore) =>
                            IfExistUpd(nomeoperatore, codiceoperatore, sp.OperatoreEsisteNicknameUpd);

        public bool IfExistBadge(OperatoreMap dT) => IfExist(sp.OperatoreEsisteBadge, dT.Badge);
        public bool IfExistBadge(int badge, int codiceoperatore) =>
                            IfExistUpd(badge, codiceoperatore, sp.OperatoreEsisteBadgeUpd);

        public bool IfExistPermessi(OperatoreMap dT) => IfExist(sp.PermessoEsisteOperatore, dT.Id);

        public int Add(OperatoreMap map)
        {
            SetDbParam(sp.OperatoreInsertData);

            Cmd.Parameters.Add(new SqlParameter("@nomeoperatore", map.NomeOperatore));
            Cmd.Parameters.Add(new SqlParameter("@password", map.Password));
            Cmd.Parameters.Add(new SqlParameter("@abilitato", map.Abilitato));
            Cmd.Parameters.Add(new SqlParameter("@pass", map.Badge));

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
                Conn = null;
                Cmd = null;
            }


            return result;
        }
        public bool Del(OperatoreMap map) => Del(map.Id, sp.OperatoreDeleteData);
        public bool Upd(OperatoreMap map)
        {
            SetDbParam(sp.OperatoreUpdateData);

            Cmd.Parameters.Add(new SqlParameter("@id", map.Id));
            Cmd.Parameters.Add(new SqlParameter("@nomeoperatore", map.NomeOperatore));
            Cmd.Parameters.Add(new SqlParameter("@password", map.Password));
            Cmd.Parameters.Add(new SqlParameter("@abilitato", map.Abilitato));
            Cmd.Parameters.Add(new SqlParameter("@pass", map.Badge));

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
