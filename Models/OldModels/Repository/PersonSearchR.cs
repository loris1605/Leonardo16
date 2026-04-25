using Microsoft.Data.SqlClient;
using Models.Entity;
using Models.Interfaces;
using Models.StoreProcedure;
using SysNet;
using System.Data;
using System.Windows;

namespace Models.Repository
{
    public class PersonSearchR : RepositoryOld, IDisposable, IPersonSearchR
    {
        public PersonSearchR() : base() { }
        public PersonSearchR(string connectionstring) : base(connectionstring) { }

        private readonly PersonSP sp = Create<PersonSP>.Instance();

        static int deadentries;

        ~PersonSearchR()
        {
            Interlocked.Increment(ref deadentries);
            _ = DisposeR.WriteDisposeAsync(this.GetType().Name + " #" + deadentries.ToString());

        }

        public override void Dispose() => base.Dispose();

        public int FirstIdByNumeroTessera(int numerotessera) => GetId(numerotessera, sp.PersonGetIdByNumeroTessera);

        public int FirstIdByNumeroSocio(string numerosocio) => GetId(numerosocio, sp.PersonGetIdByNumeroSocio);

        private List<PersonMap> LoadString(string param, string procedure)
        {
            SetDbParam(procedure);
            Cmd.Parameters.Add(new SqlParameter("@param1", (string)param));

            List<PersonMap> result = Create<List<PersonMap>>.Instance();
            try
            {
                Conn.Open();
                using SqlDataReader rdr = Cmd.ExecuteReader(CommandBehavior.CloseConnection);
                result = ToList<PersonMap>(rdr);

            }
            catch (Exception)
            {
                MessageBox.Show("Errore Procedura " + procedure);
            }
            finally
            {
                Conn.Close();
                Conn = null;
                Cmd = null;
            }

            return result;
        }
        public List<PersonMap> LoadByCognome(string cognome) => LoadString(cognome, sp.PersonGetByCognome);
        public List<PersonMap> LoadByNome(string nome) => LoadString(nome, sp.PersonGetByNome);
        public List<PersonMap> LoadStartByCognome(string cognome) => LoadString(cognome, sp.PersonGetByCognomeStartWith);
        public List<PersonMap> LoadContainsCognome(string cognome) => LoadString(cognome, sp.PersonGetByCognomeContains);
        public List<PersonMap> LoadStartByNome(string nome) => LoadString(nome, sp.PersonGetByNomeStartWith);
        public List<PersonMap> LoadContainsNome(string nome) => LoadString(nome, sp.PersonGetByNomeContains);

        private List<PersonMap> LoadByInt(int param, string procedure)
        {
            SetDbParam(procedure);
            Cmd.Parameters.Add(new SqlParameter("@param1", param));

            List<PersonMap> result = Create<List<PersonMap>>.Instance();
            try
            {
                Conn.Open();
                using SqlDataReader rdr = Cmd.ExecuteReader(CommandBehavior.CloseConnection);
                result = ToList<PersonMap>(rdr);

            }
            catch (Exception)
            {
                MessageBox.Show("Errore Procedura " + procedure);
            }
            finally
            {
                Conn.Close();
                Conn = null;
                Cmd = null;
            }

            return result;
        }
        public List<PersonMap> LoadByNato(int natoil) => LoadByInt(natoil, sp.PersonGetByNatoil);
        public List<PersonMap> LoadMinorNato(int natoil) => LoadByInt(natoil, sp.PersonGetMinorThanNatoil);
        public List<PersonMap> LoadMaiorNato(int natoil) => LoadByInt(natoil, sp.PersonGetMaiorThanNatoil);


    }
}
