using Microsoft.Data.SqlClient;
using SysNet;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace Models.Repository
{
    public abstract class RepositoryOld : IDisposable
    {
        public RepositoryOld()
        {
            Str = Connection.CurrentConnectionString;
        }

        protected int _deadEntries;

#if DEBUG
        ~RepositoryOld()
        {
            // Questo apparirà nella finestra "Output" di Visual Studio
            Debug.WriteLine($"***** [GC] {this.GetType().Name} " +
                            $"#{_deadEntries} DISTRUTTO *****");
        }
#endif

        public RepositoryOld(string connectionstring)
        {
            Str = connectionstring;
        }

        public string Str { get; set; }
        protected SqlConnection Conn { get; set; }
        protected SqlCommand Cmd { get; set; }

        public virtual void Dispose()
        {
            //GC.SuppressFinalize(this);
        }

        protected void SetDbParam(string procedure)
        {
            Conn = Create<SqlConnection>.Instance();
            Conn.ConnectionString = Str;
            Cmd = Conn.CreateCommand();
            Cmd.CommandType = System.Data.CommandType.StoredProcedure;
            Cmd.CommandText = procedure;
        }

        public virtual List<T> ToList<T>(SqlDataReader rdr)
        {
            List<T> ret = [];
            T entity;
            Type typ = typeof(T);
            PropertyInfo col;
            List<PropertyInfo> columns = [];
            // Get all the properties in Entity Class
            PropertyInfo[] props = typ.GetProperties();
            // Loop through one time to map columns to properties
            // NOTES:
            // Assumes your column names are the same name
            // as your class property names
            // Any properties not in the data reader column list are not set
            for (int index = 0; index < rdr.FieldCount; index++)
            {
                // See if column name maps directly to property name
                col = props.FirstOrDefault(c => c.Name == rdr.GetName(index));
                if (col != null)
                {
                    columns.Add(col);
                }
            }
            // Loop through all records
            while (rdr.Read())
            {
                // Create new instance of Entity
                entity = Activator.CreateInstance<T>();
                // Loop through columns to assign data
                for (int i = 0; i < columns.Count; i++)
                {
                    if (rdr[columns[i].Name].Equals(DBNull.Value))
                    {
                        columns[i].SetValue(entity, null, null);
                    }
                    else
                    {
                        columns[i].SetValue(entity, rdr[columns[i].Name], null);
                    }
                }
                ret.Add(entity);
            }

            return ret;
        }

        
        public bool IfExist(string procedure, object value = null)
        {
            SetDbParam(procedure);
            if (value != null)
            {
                Cmd.Parameters.Add(new SqlParameter("@param1", value));
            }

            bool result = false;

            try
            {
                Conn.Open();
                result = (bool)Cmd.ExecuteScalar();
            }
            catch (Exception)
            {
                MessageBox.Show("Errore Procedura " + procedure);
                result = true;

            }
            finally { Conn.Close(); }

            return result;
        }

        public bool IfExistUpd(object value, int id, string procedure)
        {
            SetDbParam(procedure);
            Cmd.Parameters.Add(new SqlParameter("@param1", value));
            Cmd.Parameters.Add(new SqlParameter("@id", id));
            bool result = false;

            try
            {
                Conn.Open();
                result = (bool)Cmd.ExecuteScalar();
            }
            catch (Exception)
            {
                MessageBox.Show("Errore Procedura " + procedure);
                result = true;

            }
            finally { Conn.Close(); }

            return result;
        }

        public virtual bool Del(int id, string procedure)
        {
            SetDbParam(procedure);
            Cmd.Parameters.Add(new SqlParameter("@param1", id));

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

        public virtual int GetId(object param1, string procedure)
        {
            SetDbParam(procedure);
            Cmd.Parameters.Add(new SqlParameter("@param1", param1));

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
            catch (Exception) { MessageBox.Show("Errore Procedura " + procedure); }
            finally
            {
                Conn.Close();
                Conn = null;
                Cmd = null;
            }

            return result;
        }

        

        public virtual List<T> GetData<T>(string procedure, object inputparam = null, object inputparam2 = null)
        {
            using SqlConnection Conn = Create<SqlConnection>.Instance();
            Conn.ConnectionString = Str;
            using SqlCommand Cmd = Conn.CreateCommand();
            Cmd.CommandType = System.Data.CommandType.StoredProcedure;
            Cmd.CommandText = procedure;
            List<T> result = Create<List<T>>.Instance();

            if (inputparam != null)
            {
                Cmd.Parameters.Add(new SqlParameter("@param1", inputparam));
                if (inputparam2 != null)
                {
                    Cmd.Parameters.Add(new SqlParameter("@param2", inputparam2));
                }
            }

            try
            {
                Conn.Open();
                using SqlDataReader rdr = Cmd.ExecuteReader(CommandBehavior.CloseConnection);
                result = ToList<T>(rdr);
                rdr.Close();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
                MessageBox.Show(ex.InnerException.Message);
                MessageBox.Show("Errore Procedura " + procedure);
            }
            finally { Conn.Close(); }

            return result;
        }

        public virtual async Task<List<T>> GetDataAsync<T>(string procedure,
                                                   object inputparam = null,
                                                   object inputparam2 = null,
                                                   CancellationToken ct = default)
        {
            // Usiamo 'using' per gestire automaticamente la chiusura
            using SqlConnection conn = new (Str);
            using SqlCommand cmd = conn.CreateCommand();

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = procedure;

            if (inputparam != null)
            {
                cmd.Parameters.Add(new SqlParameter("@param1", inputparam));
                if (inputparam2 != null)
                {
                    cmd.Parameters.Add(new SqlParameter("@param2", inputparam2));
                }
            }

            try
            {
                // Apertura asincrona della connessione
                await conn.OpenAsync(ct);

                // Esecuzione asincrona del reader
                using SqlDataReader rdr = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection, ct);

                // Trasformiamo anche ToList in asincrono per non bloccare la UI durante il mapping
                return await ToListAsync<T>(rdr, ct);
            }
            catch (SqlException ex)
            {
                // In ReactiveUI è meglio rilanciare l'eccezione e gestirla nel ViewModel via .ThrownExceptions
                Debug.WriteLine($"Errore SQL in {procedure}: {ex.Message}");
                throw;
            }
        }

        // Metodo di supporto asincrono per la mappatura
        protected virtual async Task<List<T>> ToListAsync<T>(SqlDataReader rdr, CancellationToken ct)
        {
            List<T> ret = [];
            var props = typeof(T).GetProperties();

            // Leggi i dati riga per riga in modo asincrono
            while (await rdr.ReadAsync(ct))
            {
                T entity = Activator.CreateInstance<T>();
                for (int i = 0; i < rdr.FieldCount; i++)
                {
                    string colName = rdr.GetName(i);
                    var prop = props.FirstOrDefault(p => p.Name == colName);

                    if (prop != null && !await rdr.IsDBNullAsync(i, ct))
                    {
                        prop.SetValue(entity, rdr.GetValue(i));
                    }
                }
                ret.Add(entity);
            }
            return ret;
        }

    }
}
