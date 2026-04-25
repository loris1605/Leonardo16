using Microsoft.Data.SqlClient;
using System.Data;

namespace SysNet
{
    public class Connection 
    {
        public static string CurrentConnectionString => Settings.Default.ConnectionString;

        public static void TestConnection()
        {
            using SqlConnection connection = new(CurrentConnectionString);
            try
            {
                connection.Open();
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                    Flags.ServerAttivo = true;

                }
                else { Flags.ServerAttivo = false; }
            }
            catch { Flags.ServerAttivo = false; }

        }

        public static bool SetConnectionString(string connectionstring)
        {
            if (connectionstring == null) { return false; }
            Settings.Default.ConnectionString = connectionstring;
            Settings.Default.Save();
            return true;

        }
    }
}
