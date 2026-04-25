namespace Models.StoreProcedure
{
    public class PersonSP
    {
        public string PersonMapGetData => "PersonMapGetData";
        public string PersonMapGetById => "PersonMapGetById";
        public string SocioAnyByPerson => "SocioAnyByPerson";
        public string PersonInsertData => "PersonInsertData";
        public string PersonDeleteData => "PersonDeleteData";
        public string PersonUpdateData => "PersonUpdateData";
        public string PersonEsisteCodiceUnivoco => "PersonEsisteCodiceUnivoco";
        public string PersonEsisteCodiceUnivocoUpd => "PersonEsisteCodiceUnivocoUpd";
        public string PersonGetIdByNumeroTessera => "PersonGetIdByNumeroTessera";
        public string PersonGetIdByNumeroSocio => "PersonGetIdByNumeroSocio";
        public string PersonGetByCognome => "PersonGetByCognome";
        public string PersonGetByNome => "PersonGetByNome";
        public string PersonGetByCognomeStartWith => "PersonGetByCognomeStartWith";
        public string PersonGetByCognomeContains => "PersonGetByCognomeContains";
        public string PersonGetByNomeStartWith => "PersonGetByNomeStartWith";
        public string PersonGetByNomeContains => "PersonGetByNomeContains";
        public string PersonGetByNatoil => "PersonGetByNatoil";
        public string PersonGetMinorThanNatoil => "PersonGetMinorThanNatoil";
        public string PersonGetMaiorThanNatoil => "PersonGetMaiorThanNatoil";
        public string SocioEsisteNumero => "SocioEsisteNumero";
        public string SociGet => "SociGet";
        public string SocioGetById => "SocioGetById";
        public string TesseraAnyBySocio => "TesseraAnyBySocio";
        public string SocioEsisteNumeroSocioUpd => "SocioEsisteNumeroSocioUpd";
        public string SocioInsertData => "SocioInsertData";
        public string SocioDeleteData => "SocioDeleteData";
        public string SocioUpdateData => "SocioUpdateData";
        public string TesseraEsisteNumero => "TesseraEsisteNumero";
        public string TesseraEsisteNumeroUpd => "TesseraEsisteNumeroUpd";
        public string TesseraGetById => "TesseraGetById";
        public string TesseraInsertData => "TesseraInsertData";
        public string TesseraDeleteData => "TesseraDeleteData";
        public string TesseraUpdateData => "TesseraUpdateData";
    }
}
