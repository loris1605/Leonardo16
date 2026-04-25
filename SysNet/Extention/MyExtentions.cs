using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;

namespace SysNet.Pippo
{
    public static class MyExtentions
    {
        public static bool LongThan2Chars(this string str)
        {
            return str != null && str.Length >= 2;
        }

       
    }
}
