namespace SysNet
{
    public class Flags
    {
        public static bool ServerAttivo { get; set; }

        public static DatabaseAttivo DbStatus { get; set; }


        public enum DatabaseAttivo
        {
            NoDataBase = 0,
            SiDataBase = 1,
            DaAggiornare = 2

        }


    }
}
