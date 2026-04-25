using Models.Entity;
using Models.Entity.Global;
using Models.StoreProcedure;
using SysNet;

namespace Models.Repository
{
    public partial class LoginROld : RepositoryOld, IDisposable, ILoginR
    {
        public LoginROld() : base()
        {
            System.Diagnostics.Debug.WriteLine($"***** [VM] {this.GetType().Name} " +
                                               $"#{Interlocked.Increment(ref classCount)} caricato *****");
            base._deadEntries = classCount;
        }
        public LoginROld(string connectionstring) : base(connectionstring) { }

        private static int classCount;

        private readonly LoginSP sp = Create<LoginSP>.Instance();

        public override void Dispose() => base.Dispose();


        public List<LoginMap> GetOperatoriAbilitati() => GetData<LoginMap>(sp.OperatoriGetAbilitati);
        public async Task<List<LoginMap>> GetOperatoriAbilitatiAsync()
        {
            return await GetDataAsync<LoginMap>(sp.OperatoriGetAbilitati);
        }


        public void SaveSettings(LoginMap dT)
        {
            //OperatoreXC XOperatore = Create<OperatoreXC>.Instance();

            //XOperatore.IDOPERATORE = dT.Id;
            //XOperatore.NOMEOPERATORE = dT.NomeOperatore;
            //XOperatore.PASSWORD = dT.Password;
            //XOperatore.POSTAZIONI = ListPostazioniByOperatore(dT.Id);



            //if (XOperatore.POSTAZIONI == null)
            //{
            //    GlobalValuesC.MySetting = XOperatore;
            //    return;
            //}

            //for (int x = 0; x < XOperatore.POSTAZIONI.Count; x += 1)
            //{
            //    XOperatore.POSTAZIONI[x].SETTORI = SelectSettoriX(XOperatore.POSTAZIONI[x].CODICEPOSTAZIONE);

            //    if (XOperatore.POSTAZIONI[x].SETTORI != null)
            //    {
            //        for (int y = 0; y < XOperatore.POSTAZIONI[x].SETTORI.Count; y += 1)
            //        {

            //            XOperatore.POSTAZIONI[x].SETTORI[y].TARIFFE =
            //                SelectTariffeX(XOperatore.POSTAZIONI[x].SETTORI[y].CODICESETTORE);
            //        }
            //    }

            //}

            //XOperatore.GIORNATA = GetGiornataOpen();
            //GlobalValuesC.MySetting = XOperatore;

            //if (gT == null)
            //{
            //    XOperatore.GIORNATA = null;
            //}
            //else
            //{
            //    XOperatore.GIORNATA = gT;
            //}

            //GlobalValuesC.MySetting = XOperatore;

        }

        //private List<PostazioneXC> ListPostazioniByOperatore(int CodiceOperatore)
        //{
        //    List<PostazioneXC> qx = [];
        //    var per = GetData<PermessoMap>(sp.PostazioniGetByOperatore, CodiceOperatore);

        //    if (per is null || per.Count == 0) return null;

        //    foreach (PermessoMap Element in per)
        //    {
        //        PostazioneXC qr = new()
        //        {
        //            CODICEPOSTAZIONE = Element.CodicePostazione,
        //            DESCPOSTAZIONE = Element.NomePostazione,
        //            TIPOPOSTAZIONE = Element.CodiceTipoPostazione
        //        };
        //        qx.Add(qr);
        //    }
        //    return qx;

        //}

        //private List<SettoreXC> SelectSettoriX(int CodicePostazione)
        //{
        //    List<SettoreXC> qx = [];
        //    var per = GetData<RepartoMap>(sp.SettoriGetByPostazione, CodicePostazione);

        //    if (per is null || per.Count == 0) return null;

        //    foreach (RepartoMap Element in per)
        //    {
        //        SettoreXC qr = new()
        //        {
        //            CODICESETTORE = Element.CodiceSettore,
        //            DESCSETTORE = Element.EtichettaSettore,
        //            NOMESETTORE = Element.NomeSettore
        //        };
        //        qx.Add(qr);
        //    }
        //    return qx;

        //}

        //private List<TariffaXC> SelectTariffeX(int CodiceSettore)
        //{
        //    List<TariffaXC> qx = [];
        //    var per = GetData<ListinoMap>(sp.TariffeGetBySettore, CodiceSettore);

        //    if (per is null || per.Count == 0) return null;

        //    foreach (ListinoMap Element in per)
        //    {
        //        TariffaXC qr = new()
        //        {
        //            CODICETARIFFA = Element.CodiceTariffa,
        //            DESCTARIFFA = Element.EtichettaTariffa,
        //            PRICETARIFFA = Element.PrezzoTariffa
        //        };
        //        qx.Add(qr);
        //    }
        //    return qx;

        //}

        //private GiornataXC GetGiornataOpen()
        //{
        //    var per = GetData<GiornataXC>(sp.GiornataGetAperta);
        //    if (per is null || per.Count == 0) return null;
        //    return per[0];
        //}
    }
}
