using Microsoft.Data.SqlClient;
using SysNet;
using System.Windows;

namespace Models.Repository
{
    public class DisposeR : RepositoryOld
    {
        public DisposeR() : base() { }

        public DisposeR(string connectionstring) : base(connectionstring) { }

        
        public override void Dispose()
        {
            base.Dispose();
            GC.SuppressFinalize(this);
        }

        public static void WriteDispose(string classname)
        {
            using SqlConnection conn = Create<SqlConnection>.Instance();
            conn.ConnectionString = Connection.CurrentConnectionString;
            using SqlCommand cmd = conn.CreateCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "DisposedInsert";

            cmd.Parameters.Add(new SqlParameter("@param1", classname));

            try
            {
                conn.Open();
                cmd.ExecuteNonQuery();

            }
            catch (Exception)
            {
                MessageBox.Show("Errore Scrittura Tabella Disposed");

            }
            finally
            {
                conn.Close();
            }
        }

        public static async Task WriteDisposeAsync(string classname)
        {
            await Task.Run(() =>
            {
                using SqlConnection conn = Create<SqlConnection>.Instance();
                conn.ConnectionString = Connection.CurrentConnectionString;
                using SqlCommand cmd = conn.CreateCommand();
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = "DisposedInsert";
                cmd.Parameters.Add(new SqlParameter("@param1", classname));

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();

                }
                catch (Exception)
                {
                    //MessageBox.Show("Errore Scrittura Tabella Disposed");

                }
                finally
                {
                    conn.Close();
                }
            });

        }
    }
}
