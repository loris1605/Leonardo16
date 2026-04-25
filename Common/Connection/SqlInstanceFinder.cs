using Microsoft.Data.Sql;
using System.Data;

namespace SysNet
{
    public static class SqlInstanceFinder
    {
        public static List<string> GetInstances()
        {
            var result = new List<string>();

            DataTable table = SqlDataSourceEnumerator.Instance.GetDataSources();

            foreach (DataRow row in table.Rows)
            {
                string server = row["ServerName"].ToString();
                string instance = row["InstanceName"]?.ToString();

                result.Add(string.IsNullOrWhiteSpace(instance)
                    ? server
                    : $"{server}\\{instance}");
            }

            return result;
        }
    }
}
