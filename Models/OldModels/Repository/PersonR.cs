using Microsoft.Data.SqlClient;
using Models.Entity;
using Models.StoreProcedure;
using SysNet;
using System.Data;

namespace Models.Repository
{
    public partial class PersonR : RepositoryOld, IPersonR
    {
        public PersonR() : base() { }
        public PersonR(string connectionstring) : base(connectionstring) { }

        private readonly PersonSP sp = Create<PersonSP>.Instance();

        static int deadentries;

#if DEBUG
        ~PersonR()
        {
            Interlocked.Increment(ref deadentries);
            _ = DisposeR.WriteDisposeAsync(this.GetType().Name + " #" + deadentries.ToString());
            //MessageBox.Show("PersonR Destroyed");
        }
#endif

        public override void Dispose() => base.Dispose();


        public List<PersonMap> Load(int index = 0) =>
                        index == 0 ?
                            GetData<PersonMap>(sp.PersonMapGetData) :
                            GetData<PersonMap>(sp.PersonMapGetById, index);


        public List<PersonMap> LoadByModel(object model) => (List<PersonMap>)model;

        public PersonMap First(int id)
        {
            List<PersonMap> list = Load(id);
            if (list == null || list.Count == 0) return null;
            return list[0];
        }

        public bool HasCodiciSocio(int idperson) => IfExist(sp.SocioAnyByPerson, idperson);


        public int Add(PersonMap map)
        {
            SetDbParam(sp.PersonInsertData);

            Cmd.Parameters.Add(new SqlParameter("@firstname", map.Nome));
            Cmd.Parameters.Add(new SqlParameter("@surname", map.Cognome));
            Cmd.Parameters.Add(new SqlParameter("@natoil", map.NatoilDate.DateToInt()));
            Cmd.Parameters.Add(new SqlParameter("@uniqueparam", map.CodiceUnivoco));

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
        public bool Del(int id) => Del(id, sp.PersonDeleteData);

        public bool Upd(PersonMap map)
        {
            SetDbParam(sp.PersonUpdateData);

            Cmd.Parameters.Add(new SqlParameter("@id", map.Id));
            Cmd.Parameters.Add(new SqlParameter("@firstname", map.Nome));
            Cmd.Parameters.Add(new SqlParameter("@surname", map.Cognome));
            Cmd.Parameters.Add(new SqlParameter("@natoil", map.NatoilDate.DateToInt()));
            Cmd.Parameters.Add(new SqlParameter("@uniqueparam", map.CodiceUnivoco));

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

        public bool EsisteCodiceUnivoco(string codiceunivoco) => IfExist(sp.PersonEsisteCodiceUnivoco, codiceunivoco);
        public bool EsisteCodiceUnivoco(string codiceunivoco, int idperson) =>
                             IfExistUpd(codiceunivoco, idperson, sp.PersonEsisteCodiceUnivocoUpd);

    }


}
